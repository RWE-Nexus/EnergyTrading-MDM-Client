namespace EnergyTrading.Mdm.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Runtime.Caching;
    using EnergyTrading.Caching;
    using EnergyTrading.Contracts.Search;
    using EnergyTrading.Logging;
    using EnergyTrading.Mdm.Client.Extensions;
    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;
    using EnergyTrading.Search;

    /// <summary>
    /// Caching <see cref="IMdmEntityService{T}"/> that uses <see cref="CacheItemPolicy"/> to control behaviour
    /// </summary>
    public class CachePolicyMdmEntityService<TContract> : IMdmEntityService<TContract>
        where TContract : class, IMdmEntity
    {
        private readonly static ILogger Logger = LoggerFactory.GetLogger(typeof(CachePolicyMdmEntityService<TContract>));

        private readonly IMdmEntityService<TContract> service;
        private readonly IMdmClientCacheService cache;
        private readonly IMdmClientCacheService searchCache;
        private readonly ICacheItemPolicyFactory cacheItemPolicyFactory;
        private readonly Dictionary<int, string> etags;
        private readonly string entityName;
        private readonly object syncLock;
        private readonly IMdmClientCacheRepository cacheRepository;
        private readonly uint contractVersion;

        /// <summary>
        /// Create a new instance of the <see cref="CachePolicyMdmEntityService{T}" /> class.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="cacheItemPolicyFactory"></param>
        public CachePolicyMdmEntityService(IMdmEntityService<TContract> service, ICacheItemPolicyFactory cacheItemPolicyFactory,IMdmClientCacheRepository mdmCacheRepository, uint version = 0)
        {
            this.service = service;
            this.cacheName = typeof(TContract).GetCacheNameFromEntityType(version);
            this.cacheRepository = mdmCacheRepository;
            this.cache = cacheRepository.GetNamedCache(cacheName);
            this.searchCache = cacheRepository.GetNamedCache("MDMSearchResultsCache");
            this.cacheItemPolicyFactory = cacheItemPolicyFactory;
            this.etags = new Dictionary<int, string>();
            this.syncLock = new object();
            this.entityName = typeof(TContract).Name;
            this.contractVersion = version;
        }

        private string cacheName { get; set; }

        public int Count
        {
            get { return 0; }
        }

        public int MappingCount
        {
            get { return 0; }
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
                WebResponse<TContract> response;

                if (entity != null)
                {
                    response = new WebResponse<TContract> { Message = entity, Code = HttpStatusCode.OK, IsValid = true };
                    response.LogResponse();
                }
                else
                {
                    response = this.AcquireEntity(id, () => this.service.Get(id, validAt));
                }

                return response;
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

                var entity= cache.Get<TContract>(identifier);
                if (entity != null)
                {
                    var response =  new WebResponse<TContract> { Code = HttpStatusCode.OK, Message = entity, IsValid = true };
                    response.LogResponse();
                    return response;
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
            return Create(contract, null);
        }

        /// <copydocfrom cref="IMdmEntityService{T}.CreateMapping(int, MdmId)" />
        public WebResponse<MdmId> CreateMapping(int id, MdmId identifier)
        {
            return CreateMapping(id, identifier, null);
        }

        /// <copydocfrom cref="IMdmEntityService{T}.DeleteMapping(int, int)" />
        public WebResponse<TContract> DeleteMapping(int entityEntityId, int mappingId)
        {
            return DeleteMapping(entityEntityId, mappingId, null);
        }

        /// <copydocfrom cref="IMdmEntityService{T}.GetMapping(int, Predicate{MdmId})" />
        public WebResponse<MdmId> GetMapping(int id, Predicate<MdmId> query)
        {
            var response = this.Get(id);
            WebResponse<MdmId> webResponse;
            if (response.IsValid)
            {
                webResponse = new WebResponse<MdmId>
                {
                    Code = HttpStatusCode.OK,
                    Message = response.Message.Identifiers.FirstOrDefault(mapping => query(mapping)),
                    IsValid = true
                };
            }
            else
            {
                webResponse =  new WebResponse<MdmId>
                {
                    Code = response.Code,
                    Fault = response.Fault,
                    IsValid = false
                };
            }

            webResponse.LogResponse();

            return webResponse;
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

                cache.Remove(id);

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
                        this.searchCache.Add(key, result, this.cacheItemPolicyFactory.CreatePolicy());
                    }
                }
            }
            else
            {
                // When result is null - then service will be called - which logs the response. 
                // As search results in a List - checking to avoid logging duplicate response
                result.LogResponse();
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
            return Update(id, contract, etag, null);
        }

        public WebResponse<TContract> Create(TContract contract, MdmRequestInfo requestInfo)
        {
            try
            {
                Logger.DebugFormat("Start: Create<{0}>", this.entityName);
                var response = this.service.Create(contract, requestInfo);
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

        public WebResponse<MdmId> CreateMapping(int id, MdmId identifier, MdmRequestInfo requestInfo)
        {
            try
            {
                Logger.DebugFormat("Start: CreateMapping<{0}>: {1}", this.entityName, identifier);
                return this.service.CreateMapping(id, identifier, requestInfo);
            }
            finally
            {
                Logger.DebugFormat("Stop: CreateMapping<{0}>: {1}", this.entityName, identifier);
            }
        }

        public WebResponse<TContract> DeleteMapping(int entityId, int mappingId, MdmRequestInfo requestInfo)
        {
            try
            {
                Logger.DebugFormat("Start: DeleteMapping<{0}>: {1} - {2}", this.entityName, entityId, mappingId);
                return this.service.DeleteMapping(entityId, mappingId, requestInfo);
            }
            finally
            {
                Logger.DebugFormat("Stop: DeleteMapping<{0}>: {1} - {2}", this.entityName, entityId, mappingId);
            }
        }

        public WebResponse<TContract> Update(int id, TContract contract, MdmRequestInfo requestInfo)
        {
            return Update(id, contract, this.etags[id], requestInfo);
        }

        public WebResponse<TContract> Update(int id, TContract contract, string etag, MdmRequestInfo requestInfo)
        {
            try
            {
                Logger.DebugFormat("Start: Update<{0}> - {1}", this.entityName, id);
                var response = this.service.Update(id, contract, etag,requestInfo);
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
                    response =  new WebResponse<TContract>
                    {
                        Code = HttpStatusCode.OK,
                        Message = entity,
                        IsValid = true
                    };

                    response.LogResponse();

                    return response;
                }

                // Otherwise process the response from the web call
                if (response.IsValid)
                {
                    this.ProcessContract(response);
                }
            }

            response.LogResponse();
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
                var entity = cache.Get<TContract>(sourceIdentifier);
                if (entity != null)
                {
                    var webResponse =  new WebResponse<TContract> { Code = HttpStatusCode.OK, Message = entity, IsValid = true };
                    webResponse.LogResponse();
                    return webResponse;
                }

                // Otherwise process the response from the web call
                if (response.IsValid)
                {
                    this.ProcessContract(response);
                }
            }
            response.LogResponse();
            return response;
        }

        private TContract CheckCache(int id)
        {
            return this.cache.Get<TContract>(id);
        }

        private PagedWebResponse<IList<TContract>> CheckSearchCache(string key)
        {
            return this.searchCache.Get<PagedWebResponse<IList<TContract>>>(key);
        }

        private string ToSearchKey(Search search)
        {
            return "client" + search.ToKey<TContract>(contractVersion);
        }

        private void ProcessContract(WebResponse<TContract> reponse)
        {
            try
            {
                var entity = reponse.Message;
                var id = entity.ToMdmKey();

                lock (this.syncLock)
                {
                    this.cache.Add(entity, this.cacheItemPolicyFactory.CreatePolicy());
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