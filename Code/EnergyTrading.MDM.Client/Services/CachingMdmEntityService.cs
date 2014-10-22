namespace EnergyTrading.Mdm.Client.Services
{
    using EnergyTrading.Contracts.Search;
    using EnergyTrading.Logging;
    using EnergyTrading.Mdm.Client.Extensions;
    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    /// <summary>
    /// Caching implementation of <see cref="IMdmEntityService{T}" />, allows us to control the cache at an entity level.
    /// </summary>
    /// <typeparam name="TContract"></typeparam>
    public class CachingMdmEntityService<TContract> : IMdmEntityService<TContract>
        where TContract : class, IMdmEntity
    {
        private readonly static ILogger Logger = LoggerFactory.GetLogger(typeof(CachingMdmEntityService<TContract>));

        private readonly IMdmEntityService<TContract> service;
        private readonly Dictionary<int, TContract> entities;
        private readonly Dictionary<int, string> etags;
        private readonly Dictionary<MdmId, int> mappings;
        private readonly object syncLock;
        private readonly string entityName;

        public CachingMdmEntityService(IMdmEntityService<TContract> service)
        {
            this.service = service;
            this.mappings = new Dictionary<MdmId, int>();
            this.entities = new Dictionary<int, TContract>();
            this.etags = new Dictionary<int, string>();
            this.syncLock = new object();
            this.entityName = typeof(TContract).Name;
        }

        /// <summary>
        /// Gets the count of entities in the cache.
        /// </summary>
        public int Count
        {
            get { return this.entities.Count; }
        }

        /// <summary>
        /// Gets the count of mappings in the cache.
        /// </summary>
        public int MappingCount
        {
            get { return this.mappings.Count; }
        }

        /// <summary>
        /// Clear the cache.s
        /// </summary>
        public void Clear()
        {
            Logger.Debug("Start: CachingMdmEntityService.Clear : Clearing Cache");
            lock (this.syncLock)
            {
                this.etags.Clear();
                this.entities.Clear();
                this.mappings.Clear();
            }
            Logger.Debug("Stop : CachingMdmEntityService.Clear");
        }

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
                    Logger.DebugFormat("Start CachingMdmEntityService.GetList<{0}>: {1} - {2}", this.entityName, id);
                }

                return this.service.GetList(id);
            }
            finally
            {
                if (Logger.IsDebugEnabled)
                {
                    Logger.DebugFormat("Stop CachingMdmEntityService.GetList<{0}>: {1} - {2}", this.entityName, id);
                }
            }
        }

        public WebResponse<TContract> Get(int id, DateTime? validAt)
        {
            try
            {
                Logger.DebugFormat("Start: CachingMdmEntityService.Get<{0}> {1} asOf {2}", this.entityName, id, validAt);
                TContract entity;
                Logger.DebugFormat("CachingMdmEntityService.Get : Checking {0} in cache", id);
                if (this.entities.TryGetValue(id, out entity))
                {
                    Logger.DebugFormat("CachingMdmEntityService.Get : {0} found in cache", id);
                    var response = new WebResponse<TContract> { Message = entity };
                    response.LogResponse();
                    return response;
                }

                Logger.DebugFormat("CachingMdmEntityService.Get : {0} not found in cache. Invoking service...", id);
                return this.AcquireEntity(id, () => this.service.Get(id, validAt));
            }
            finally
            {
                Logger.DebugFormat("Stop: CachingMdmEntityService.Get : {0} asOf {1}", id, validAt);
            }
        }

        public WebResponse<TContract> Get(MdmId identifier)
        {
            // NB We push null here so that the service inteprets now and any requests can be cached upstream
            return this.Get(identifier, null);
        }

        public WebResponse<TContract> Get(MdmId identifier, DateTime? validAt)
        {
            try
            {
                Logger.DebugFormat("Start: CachingMdmEntityService.Get<{0}> {1} asOf {2}", this.entityName, identifier, validAt);
                int entityId;
                WebResponse<TContract> response;

                Logger.DebugFormat("CachingMdmEntityService.Get - Checking {0} in cache", identifier);

                if (this.mappings.TryGetValue(identifier, out entityId))
                {
                    Logger.DebugFormat("CachingMdmEntityService.Get - {0} found in cache", identifier);
                    response = new WebResponse<TContract> { Code = HttpStatusCode.OK, Message = this.entities[entityId] };
                }
                else
                {
                    Logger.DebugFormat("CachingMdmEntityService.Get - {0} not found in cache. Invoking Service...", identifier);
                    response = this.AcquireEntity(identifier, () => this.service.Get(identifier, validAt));
                }

                response.LogResponse();

                return response;
            }
            finally
            {
                Logger.DebugFormat("Stop: CachingMdmEntityService.Get {0} asOf {1}", identifier, validAt);
            }
        }

        public WebResponse<TContract> Create(TContract contract)
        {
            return Create(contract, null);
        }

        public WebResponse<MdmId> CreateMapping(int id, MdmId identifier)
        {
            return CreateMapping(id, identifier, null);
        }

        public WebResponse<TContract> DeleteMapping(int entityId, int mappingId)
        {
            return DeleteMapping(entityId, mappingId, null);
        }

        public WebResponse<MdmId> GetMapping(int id, Predicate<MdmId> query)
        {
            try
            {
                Logger.DebugFormat("Start : CachingMdmEntityService.GetMapping<{0}> - {1}", this.entityName, id);

                var response = this.Get(id);
                WebResponse<MdmId> webResponse;

                if (response.IsValid)
                {
                    webResponse = new WebResponse<MdmId>
                    {
                        Code = HttpStatusCode.OK,
                        Message = response.Message.Identifiers.FirstOrDefault(mapping => query(mapping))
                    };
                }
                else
                {
                    webResponse = new WebResponse<MdmId>
                    {
                        Code = response.Code,
                        Fault = response.Fault,
                        IsValid = false
                    };
                }

                webResponse.LogResponse();

                return webResponse;
            }
            finally
            {
                Logger.DebugFormat("Stop : CachingMdmEntityService.GetMapping<{0}> - {1}", this.entityName, id);
            }
        }

        public void Invalidate(int id)
        {
            Logger.DebugFormat("Start: CachingMdmEntityService.Invalidate {0}", id);

            try
            {
                // We could not bother locking if the id is not in the cache, but if it is present
                // we have to acquire it anyway, so take the hit - and quit quickly if we fail to match.
                lock (this.syncLock)
                {
                    TContract entity;
                    if (!this.entities.TryGetValue(id, out entity))
                    {
                        return;
                    }

                    this.entities.Remove(id);

                    foreach (var nexus in entity.Identifiers)
                    {
                        this.mappings.Remove(nexus);
                    }
                }
            }
            finally
            {
                Logger.DebugFormat("Stop: CachingMdmEntityService.Invalidate {0}", id);
            }
        }

        public WebResponse<MdmId> Map(int id, string targetSystem)
        {
            try
            {
                Logger.DebugFormat("Start: CachingMdmEntityService.Map<{0}> - '{1}' '{2}'", this.entityName, id, targetSystem);

                return this.GetMapping(
                    id,
                    ident => string.Equals(ident.SystemName, targetSystem, StringComparison.InvariantCultureIgnoreCase));
            }
            finally
            {
                Logger.DebugFormat("Stop: CachingMdmEntityService.Map<{0}> - '{1}' '{2}'", this.entityName, id, targetSystem);
            }
        }

        public WebResponse<MappingResponse> CrossMap(MdmId identifier, string targetSystem)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            return this.CrossMap(identifier.SystemName, identifier.Identifier, targetSystem);
        }

        public WebResponse<MappingResponse> CrossMap(string sourceSystem, string identifier, string targetSystem)
        {
            try
            {
                Logger.DebugFormat("Start: CachingMdmEntityService.CrossMap<{0}> - '{1}' '{2}' '{3}'", this.entityName, sourceSystem, identifier, targetSystem);
                return this.service.CrossMap(sourceSystem, identifier, targetSystem);
            }
            finally
            {
                Logger.DebugFormat("Stop: CachingMdmEntityService.CrossMap<{0}>  - '{1}' '{2}' '{3}'", this.entityName, sourceSystem, identifier, targetSystem);
            }
        }

        public PagedWebResponse<IList<TContract>> Search(Search search)
        {
            try
            {
                Logger.DebugFormat("Start : CachingMdmEntityService.Search<{0}> ", this.entityName);
                Logger.DebugFormat("CachingMdmEntityService.Search<{0}> : Calling search on Service", this.entityName);
                return this.service.Search(search);
            }
            finally
            {
                Logger.DebugFormat("Stop : CachingMdmEntityService.Search<{0}>", this.entityName);
            }
        }

        public WebResponse<TContract> Update(int id, TContract contract)
        {
            return this.Update(id, contract, this.etags[id]);
        }

        public WebResponse<TContract> Update(int id, TContract contract, string etag)
        {
            return Update(id, contract, etag, null);
        }

        public WebResponse<TContract> Create(TContract contract, MdmRequestInfo requestInfo)
        {
            try
            {
                Logger.DebugFormat("Start : CachingMdmEntityService.Create<{0}>", this.entityName);
                var response = this.service.Create(contract, requestInfo);
                if (response.IsValid)
                {
                    this.ProcessContract(response);
                }
                response.LogResponse();
                return response;
            }
            finally
            {
                Logger.DebugFormat("Stop : CachingMdmEntityService.Create<{0}>", this.entityName);
            }
        }

        public WebResponse<MdmId> CreateMapping(int id, MdmId identifier, MdmRequestInfo requestInfo)
        {
            try
            {
                Logger.DebugFormat("Start : CachingMdmEntityService.CreateMapping<{0}> - {1} {2}", this.entityName, id, identifier);
                return this.service.CreateMapping(id, identifier);
            }
            finally
            {
                Logger.DebugFormat("Stop : CachingMdmEntityService.CreateMapping<{0}>", this.entityName);
            }
        }

        public WebResponse<TContract> DeleteMapping(int entityId, int mappingId, MdmRequestInfo requestInfo)
        {
            try
            {
                Logger.DebugFormat("Start: CachingMdmEntityService.DeleteMapping<{0}> - '{1}' '{2}'", this.entityName, entityId, mappingId);
                return this.service.DeleteMapping(entityId, mappingId);
            }
            finally
            {
                Logger.DebugFormat("Stop: CachingMdmEntityService.DeleteMapping<{0}> - '{1}' '{2}'", this.entityName, entityId, mappingId);
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
                Logger.DebugFormat("Start : CachingMdmEntityService.Update<{0}> - {1}", this.entityName, id);
                var response = this.service.Update(id, contract, etag, requestInfo);
                if (response.IsValid)
                {
                    this.ProcessContract(response);
                }

                response.LogResponse();
                return response;
            }
            finally
            {
                Logger.DebugFormat("Stop : CachingMdmEntityService.Update<{0}> - {1}", this.entityName, id);
            }
        }

        private WebResponse<TContract> AcquireEntity(int id, Func<WebResponse<TContract>> finder)
        {
            try
            {
                Logger.DebugFormat("Start : CachingMdmEntityService.AcquireEntity<{0}> - {1}", this.entityName, id);
                WebResponse<TContract> response;

                // NB Problem with this is that we are single-threaded on requests
                lock (this.syncLock)
                {
                    // Check if it exist now we are in a critical section
                    TContract entity;
                    if (this.entities.TryGetValue(id, out entity))
                    {
                        response = new WebResponse<TContract>
                        {
                            Code = HttpStatusCode.OK,
                            Message = entity
                        };

                        response.LogResponse();

                        return response;
                    }

                    response = finder.Invoke();
                    if (response.IsValid)
                    {
                        this.ProcessContract(response);
                    }
                }

                return response;
            }
            finally
            {
                Logger.DebugFormat("Stop : CachingMdmEntityService.AcquireEntity<{0}> - {1}", this.entityName, id);
            }
        }

        private WebResponse<TContract> AcquireEntity(MdmId sourceIdentifier, Func<WebResponse<TContract>> finder)
        {
            try
            {
                Logger.DebugFormat("Start : CachingMdmEntityService.AcquireEntity<{0}> - {1}", this.entityName, sourceIdentifier);
                WebResponse<TContract> response;

                // NB Problem with this is that we are single-threaded on requests
                // Could break this out to a check, acquire contract via a queue, check; then process
                // this would allow more requests to be submitted,
                lock (this.syncLock)
                {
                    int entityId;
                    if (this.mappings.TryGetValue(sourceIdentifier, out entityId))
                    {
                        // Can do this as we are in a locked section
                        response = new WebResponse<TContract>
                        {
                            Code = HttpStatusCode.OK,
                            Message = this.entities[entityId]
                        };

                        response.LogResponse();

                        return response;
                    }

                    response = finder.Invoke();
                    if (response.IsValid)
                    {
                        this.ProcessContract(response);
                    }
                }

                return response;
            }
            finally
            {
                Logger.DebugFormat("Stop : CachingMdmEntityService.AcquireEntity<{0}> - {1}", this.entityName, sourceIdentifier);
            }
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
                    foreach (var identifier in entity.Identifiers)
                    {
                        if (identifier == nexus || this.mappings.ContainsKey(identifier))
                        {
                            continue;
                        }

                        this.mappings[identifier] = id;
                    }

                    this.entities[id] = entity;
                    this.etags[id] = reponse.Tag;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}