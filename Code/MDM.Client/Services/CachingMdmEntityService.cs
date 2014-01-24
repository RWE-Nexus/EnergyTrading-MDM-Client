namespace EnergyTrading.MDM.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using EnergyTrading.MDM.Client.WebClient;

    using EnergyTrading.Contracts.Search;
    using EnergyTrading.Logging;
    using RWEST.Nexus.MDM.Contracts;

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
        private readonly Dictionary<NexusId, int> mappings;
        private readonly object syncLock;

        public CachingMdmEntityService(IMdmEntityService<TContract> service)
        {
            this.service = service;
            this.mappings = new Dictionary<NexusId, int>();
            this.entities = new Dictionary<int, TContract>();
            this.etags = new Dictionary<int, string>();
            this.syncLock = new object();
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
            Logger.Debug("Start");
            lock (this.syncLock)
            {
                this.etags.Clear();
                this.entities.Clear();
                this.mappings.Clear();
            }
            Logger.Debug("Stop");
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

        public WebResponse<TContract> Get(int id, DateTime? validAt)
        {
            try
            {
                Logger.DebugFormat("Start: {0} asOf {1}", id, validAt);
                TContract entity;
                if (this.entities.TryGetValue(id, out entity))
                {
                    return new WebResponse<TContract> { Message = entity };
                }

                return this.AcquireEntity(id, () => this.service.Get(id, validAt));
            }
            finally
            {
                Logger.DebugFormat("Stop: {0} asOf {1}", id, validAt);
            }
        }

        public WebResponse<TContract> Get(NexusId identifier)
        {
            // NB We push null here so that the service inteprets now and any requests can be cached upstream
            return this.Get(identifier, null);
        }

        public WebResponse<TContract> Get(NexusId identifier, DateTime? validAt)
        {
            try
            {
                Logger.DebugFormat("Start: {0} asOf {1}", identifier, validAt);
                int entityId;
                return this.mappings.TryGetValue(identifier, out entityId) 
                    ? new WebResponse<TContract> { Code = HttpStatusCode.OK, Message = this.entities[entityId] } 
                    : this.AcquireEntity(identifier, () => this.service.Get(identifier, validAt));
            }
            finally
            {
                Logger.DebugFormat("Stop: {0} asOf {1}", identifier, validAt);
            }
        }

        public WebResponse<TContract> Create(TContract contract)
        {
            try
            {
                Logger.Debug("Start");
                var response = this.service.Create(contract);
                if (response.IsValid)
                {
                    this.ProcessContract(response);
                }

                return response;
            }
            finally
            {
                Logger.Debug("Stop");
            }
        }

        public WebResponse<NexusId> CreateMapping(int id, NexusId identifier)
        {
            try
            {
                Logger.Debug("Start");
                return this.service.CreateMapping(id, identifier);
            }
            finally
            {
                Logger.Debug("Stop");
            }
        }

        public WebResponse<TContract> DeleteMapping(int entityId, int mappingId)
        {
            try
            {
                Logger.DebugFormat("Start: '{0}' '{1}'", entityId, mappingId);
                return this.service.DeleteMapping(entityId, mappingId);
            }
            finally
            {
                Logger.DebugFormat("Stop: '{0}' '{1}'", entityId, mappingId);
            }
        }

        public WebResponse<NexusId> GetMapping(int id, Predicate<NexusId> query)
        {
            var response = this.Get(id);
            if (response.IsValid)
            {
                return new WebResponse<NexusId>
                {
                    Code = HttpStatusCode.OK,
                    Message = response.Message.Identifiers.FirstOrDefault(mapping => query(mapping))
                };
            }

            return new WebResponse<NexusId>
            {
                Code = response.Code,
                Fault = response.Fault,
                IsValid = false
            };
        }

        public void Invalidate(int id)
        {
            Logger.DebugFormat("Start: {0}", id);

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
                Logger.DebugFormat("Stop: {0}", id);  
            }
        }

        public WebResponse<NexusId> Map(int id, string targetSystem)
        {
            try
            {
                Logger.DebugFormat("Start: '{0}' '{1}'", id, targetSystem);

                return this.GetMapping(
                    id,
                    ident => string.Equals(ident.SystemName, targetSystem, StringComparison.InvariantCultureIgnoreCase));
            }
            finally
            {
                Logger.DebugFormat("Stop: '{0}' '{1}'", id, targetSystem);                
            }
        }

        public WebResponse<MappingResponse> CrossMap(NexusId identifier, string targetSystem)
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
                Logger.DebugFormat("Start: '{0}' '{1}' '{2}'", sourceSystem, identifier, targetSystem);
                return this.service.CrossMap(sourceSystem, identifier, targetSystem);
            }
            finally
            {
                Logger.DebugFormat("Stop: '{0}' '{1}' '{2}'", sourceSystem, identifier, targetSystem);
            }
        }

        public PagedWebResponse<IList<TContract>> Search(Search search)
        {
            return this.service.Search(search);
        }

        public WebResponse<TContract> Update(int id, TContract contract)
        {
            return this.Update(id, contract, this.etags[id]);
        }

        public WebResponse<TContract> Update(int id, TContract contract, string etag)
        {
            var response = this.service.Update(id, contract, etag);
            if (response.IsValid)
            {
                this.ProcessContract(response);
            }

            return response;
        }

        private WebResponse<TContract> AcquireEntity(int id, Func<WebResponse<TContract>> finder)
        {
            WebResponse<TContract> response;

            // NB Problem with this is that we are single-threaded on requests
            lock (this.syncLock)
            {
                // Check if it exist now we are in a critical section
                TContract entity;
                if (this.entities.TryGetValue(id, out entity))
                {
                    return new WebResponse<TContract>
                    {
                        Code = HttpStatusCode.OK,
                        Message = entity
                    };
                }

                response = finder.Invoke();
                if (response.IsValid)
                {
                    this.ProcessContract(response);
                }
            }

            return response;
        }

        private WebResponse<TContract> AcquireEntity(NexusId sourceIdentifier, Func<WebResponse<TContract>> finder)
        {
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
                    return new WebResponse<TContract>
                    {
                        Code = HttpStatusCode.OK,
                        Message = this.entities[entityId]
                    };
                }

                response = finder.Invoke();
                if (response.IsValid)
                {
                    this.ProcessContract(response);
                }
            }

            return response;
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