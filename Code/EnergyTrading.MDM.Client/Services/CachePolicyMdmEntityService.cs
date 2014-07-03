namespace EnergyTrading.Mdm.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Runtime.Caching;

    using EnergyTrading.Caching;
    using EnergyTrading.Contracts.Search;
    using EnergyTrading.Logging;
    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;
    using EnergyTrading.Xml.Serialization;

    /// <summary>
    /// Caching <see cref="IMdmEntityService{T}"/> that uses <see cref="CacheItemPolicy"/> to control behaviour
    /// </summary>
    public class CachePolicyMdmEntityService<TContract> : IMdmEntityService<TContract>
        where TContract : class, IMdmEntity
    {
        private readonly static ILogger Logger = LoggerFactory.GetLogger(typeof(CachePolicyMdmEntityService<TContract>));

        private readonly IMdmEntityService<TContract> service;
        private readonly ICacheService cache;
        private readonly ICacheItemPolicyFactory cacheItemPolicyFactory;
        private readonly Dictionary<MdmId, int> mappings;
        private readonly Dictionary<int, string> etags;
        private readonly string entityName;
        private readonly object syncLock;
        private readonly ICacheRepository cacheRepository;

        /// <summary>
        /// Create a new instance of the <see cref="CachePolicyMdmEntityService{T}" /> class.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="cacheItemPolicyFactory"></param>
        public CachePolicyMdmEntityService(IMdmEntityService<TContract> service, ICacheItemPolicyFactory cacheItemPolicyFactory,ICacheRepository cacheRepository, uint version = 0)
        {
            this.service = service;
            this.cacheName = "Mdm." + typeof(TContract).Name + (version > 0 ? "V" + version : string.Empty);
            this.cacheRepository = cacheRepository;
            this.cache = cacheRepository.GetNamedCache(cacheName);
            this.cacheItemPolicyFactory = cacheItemPolicyFactory;
            this.mappings = new Dictionary<MdmId, int>();
            this.etags = new Dictionary<int, string>();
            this.syncLock = new object();
            this.entityName = typeof(TContract).Name;
        }

        private string cacheName { get; set; }

        public int Count
        {
            get { return 0; }
        }

        public int MappingCount
        {
            get { return this.mappings.Count; }
        }

        /// <summary>
        /// Clear the cache.
        /// </summary>
        public void Clear()
        {
            if (cacheRepository != null && !string.IsNullOrEmpty(cacheName))
            {
                cacheRepository.RemoveNamedCache(cacheName);
            }
        }

        /// <copydocfrom cref="IMdmEntityService{T}.Get(int)" />
        public WebResponse<TContract> Get(int id)
        {
            // NB We push null here so that the service inteprets now and any requests can be cached upstream
            return this.Get(id, null);
        }

        public WebResponse<IList<TContract>> GetList(int id)
        {
            try
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.DebugFormat("Start GetList<{0}>: {1} - {2}", typeof(TContract).Name, id);
                }

                return this.service.GetList(id);
            }
            finally
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.DebugFormat("Stop GetList<{0}>: {1} - {2}", typeof(TContract).Name, id);
                }                
            }
        }

        /// <copydocfrom cref="IMdmEntityService{T}.Get(int, DateTime?)" />
        public WebResponse<TContract> Get(int id, DateTime? validAt)
        {
            try
            {
                Logger.DebugFormat("Start: Get<{0}>: {1} asOf {2}", this.entityName, id, validAt);
                var entity = this.CheckCache(id);
                if (entity != null)
                {
                    return new WebResponse<TContract> { Message = entity, Code = HttpStatusCode.OK, IsValid = true };
                }

                return this.AcquireEntity(id, () => this.service.Get(id, validAt));
            }
            finally
            {
                Logger.DebugFormat("Stop: Get<{0}>: {1} asOf {2}", this.entityName, id, validAt);
            }
        }

        /// <copydocfrom cref="IMdmEntityService{T}.Get(MdmId)" />
        public WebResponse<TContract> Get(MdmId identifier)
        {
            // NB We push null here so that the service inteprets now and any requests can be cached upstream
            return this.Get(identifier, null);
        }

        /// <copydocfrom cref="IMdmEntityService{T}.Get(int, DateTime?)" />
        public WebResponse<TContract> Get(MdmId identifier, DateTime? validAt)
        {
            try
            {
                Logger.DebugFormat("Start: Get<{0}>: {1} asOf {2}", this.entityName, identifier, validAt);

                if (identifier == null)
                {
                    throw new ArgumentNullException("identifier");
                }

                if (identifier.IsMdmId || identifier.SystemName.ToLower() == SourceSystemNames.Nexus.ToLower())
                {
                    int id;
                    if (int.TryParse(identifier.Identifier, out id))
                    {
                        return this.Get(id, validAt);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid Nexus identifier: {0}.", identifier.ToString());
                    }
                }

                int entityId;
                if (this.mappings.TryGetValue(identifier, out entityId))
                {
                    var entity = this.CheckCache(entityId);
                    if (entity != null)
                    {
                        return new WebResponse<TContract> { Code = HttpStatusCode.OK, Message = entity, IsValid = true };
                    }
                }

                return this.AcquireEntity(identifier, () => this.service.Get(identifier, validAt));
            }
            finally
            {
                Logger.DebugFormat("Stop: Get<{0}>: {1} asOf {2}", this.entityName, identifier, validAt);
            }
        }

        /// <copydocfrom cref="IMdmEntityService{T}.Create(T)" />
        public WebResponse<TContract> Create(TContract contract)
        {
            try
            {
                Logger.DebugFormat("Start: Create<{0}>", this.entityName);
                var response = this.service.Create(contract);
                if (response.IsValid)
                {
                    this.ProcessContract(response);
                }

                return response;
            }
            finally
            {
                Logger.DebugFormat("Stop: Create<{0}>", this.entityName);
            }
        }

        /// <copydocfrom cref="IMdmEntityService{T}.CreateMapping(int, MdmId)" />
        public WebResponse<MdmId> CreateMapping(int id, MdmId identifier)
        {
            try
            {
                Logger.DebugFormat("Start: CreateMapping<{0}>: {1}", this.entityName, identifier);
                return this.service.CreateMapping(id, identifier);
            }
            finally
            {
                Logger.DebugFormat("Stop: CreateMapping<{0}>: {1}", this.entityName, identifier);
            }
        }

        /// <copydocfrom cref="IMdmEntityService{T}.DeleteMapping(int, int)" />
        public WebResponse<TContract> DeleteMapping(int entityEntityId, int mappingId)
        {
            try
            {
                Logger.DebugFormat("Start: DeleteMapping<{0}>: {1} - {2}", this.entityName, entityEntityId, mappingId);
                return this.service.DeleteMapping(entityEntityId, mappingId);
            }
            finally
            {
                Logger.DebugFormat("Stop: DeleteMapping<{0}>: {1} - {2}", this.entityName, entityEntityId, mappingId);
            }
        }

        /// <copydocfrom cref="IMdmEntityService{T}.GetMapping(int, Predicate{MdmId})" />
        public WebResponse<MdmId> GetMapping(int id, Predicate<MdmId> query)
        {
            var response = this.Get(id);
            if (response.IsValid)
            {
                return new WebResponse<MdmId>
                {
                    Code = HttpStatusCode.OK,
                    Message = response.Message.Identifiers.FirstOrDefault(mapping => query(mapping)),
                    IsValid = true
                };
            }

            return new WebResponse<MdmId>
            {
                Code = response.Code,
                Fault = response.Fault,
                IsValid = false
            };
        }

        /// <copydocfrom cref="IMdmEntityService{T}.Invalidate" />
        public void Invalidate(int id)
        {
            // We could not bother locking if the id is not in the cache, but if it is present
            // we have to acquire it anyway, so take the hit - and quit quickly if we fail to match.
            lock (this.syncLock)
            {
                var entity = this.CheckCache(id);
                if (entity == null)
                {
                    return;
                }

                this.cache.Remove(id.ToString(CultureInfo.InvariantCulture));

                foreach (var nexus in entity.Identifiers)
                {
                    this.mappings.Remove(nexus);
                }
            }
        }

        /// <copydocfrom cref="IMdmEntityService{T}.Map(int, string)" />
        public WebResponse<MdmId> Map(int id, string targetSystem)
        {
            return this.GetMapping(id, ident => string.Equals(ident.SystemName, targetSystem, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <copydocfrom cref="IMdmEntityService{T}.CrossMap(MdmId, string)" />
        public WebResponse<MappingResponse> CrossMap(MdmId identifier, string targetSystem)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            return this.CrossMap(identifier.SystemName, identifier.Identifier, targetSystem);
        }

        /// <copydocfrom cref="IMdmEntityService{T}.CrossMap(MdmId, string, string)" />
        public WebResponse<MappingResponse> CrossMap(string sourceSystem, string identifier, string targetSystem)
        {
            try
            {
                Logger.DebugFormat("Start: CrossMap<{0}>: {1} - {2} - {3}", this.entityName, sourceSystem, identifier, targetSystem);
                return this.service.CrossMap(sourceSystem, identifier, targetSystem);
            }
            finally
            {
                Logger.DebugFormat("Stop: CrossMap<{0}>: {1} - {2} - {3}", this.entityName, sourceSystem, identifier, targetSystem);
            }
        }

        /// <copydocfrom cref="IMdmEntityService{T}.Search" />
        public PagedWebResponse<IList<TContract>> Search(Search search)
        {
            var key = this.ToSearchKey(search);
            Logger.DebugFormat("Searching for key: {0} within the cache", key);
            var result = this.CheckSearchCache(key);
            if (result == null)
            {
                Logger.DebugFormat("Searching for key: {0} not been found within cache", key);
                Logger.DebugFormat("calling the service to search with the key");
                result = this.service.Search(search);
                Logger.DebugFormat("Search completed with code {0}:", result.Code);

                lock (this.syncLock)
                {
                    // Check if it exist now we are in a critical section
                    var cached = this.CheckSearchCache(key);
                    if (cached != null)
                    {
                        // Return earlier result
                        result = cached;
                    }
                    else
                    {
                        Logger.DebugFormat("Adding the searched :", result.Code);
                        this.cache.Add(key, result, this.cacheItemPolicyFactory.CreatePolicy());
                    }
                }
            }

            return result;
        }

        /// <copydocfrom cref="IMdmEntityService{T}.Update(int, T)" />
        public WebResponse<TContract> Update(int id, TContract contract)
        {
            return this.Update(id, contract, this.etags[id]);
        }

        /// <copydocfrom cref="IMdmEntityService{T}.Update(int, T, string)" />
        public WebResponse<TContract> Update(int id, TContract contract, string etag)
        {
            try
            {
                Logger.DebugFormat("Start: Update<{0}> - {1}", this.entityName, id);
                var response = this.service.Update(id, contract, etag);
                if (response.IsValid)
                {
                    this.ProcessContract(response);
                }

                return response;
            }
            finally
            {
                Logger.DebugFormat("Stop: Update<{0}> - {1}", this.entityName, id);
            }
        }

        private WebResponse<TContract> AcquireEntity(int id, Func<WebResponse<TContract>> finder)
        {
            // Pretty sure we will have a cache miss if we get here, so do the OOP call outside the 
            // critical section
            var response = finder.Invoke();

            // NB Single-threaded on requests, but can't avoid that somewhere
            lock (this.syncLock)
            {
                // Check if it exist now we are in a critical section
                var entity = this.CheckCache(id);
                if (entity != null)
                {
                    return new WebResponse<TContract>
                    {
                        Code = HttpStatusCode.OK,
                        Message = entity,
                        IsValid = true
                    };
                }

                // Otherwise process the response from the web call
                if (response.IsValid)
                {
                    this.ProcessContract(response);
                }
            }

            return response;
        }

        private WebResponse<TContract> AcquireEntity(MdmId sourceIdentifier, Func<WebResponse<TContract>> finder)
        {
            // Pretty sure we will have a cache miss if we get here, so do the OOP call outside the 
            // critical section
            var response = finder.Invoke();

            // NB Single-threaded on requests, but can't avoid that somewhere 
            lock (this.syncLock)
            {
                int entityId;
                if (this.mappings.TryGetValue(sourceIdentifier, out entityId))
                {
                    // Can do this as we are in a locked section
                    var entity = this.CheckCache(entityId);
                    if (entity != null)
                    {
                        return new WebResponse<TContract> { Code = HttpStatusCode.OK, Message = entity, IsValid = true};
                    }
                }

                // Otherwise process the response from the web call
                if (response.IsValid)
                {
                    this.ProcessContract(response);
                }
            }

            return response;
        }

        private TContract CheckCache(int id)
        {
            return this.cache.Get<TContract>(id.ToString(CultureInfo.InvariantCulture));
        }

        private PagedWebResponse<IList<TContract>> CheckSearchCache(string key)
        {
            return this.cache.Get<PagedWebResponse<IList<TContract>>>(key);
        }

        private string ToSearchKey(Search search)
        {
            return search.DataContractSerialize();
        }

        private void ProcessContract(WebResponse<TContract> reponse)
        {
            try
            {
                var entity = reponse.Message;
                var nexus = entity.ToMdmId();
                var id = entity.ToMdmKey();

                lock (this.syncLock)
                {
                    this.cache.Add(id.ToString(CultureInfo.InvariantCulture), entity, this.cacheItemPolicyFactory.CreatePolicy());

                    foreach (var identifier in entity.Identifiers)
                    {
                        if (identifier == nexus || this.mappings.ContainsKey(identifier))
                        {
                            continue;
                        }

                        this.mappings[identifier] = id;
                    }

                    this.etags[id] = reponse.Tag;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}