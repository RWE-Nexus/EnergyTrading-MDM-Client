namespace EnergyTrading.Mdm.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using EnergyTrading.Contracts.Search;
    using EnergyTrading.Logging;
    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;

    public class MdmService : IMdmService
    {
        private readonly static ILogger Logger = LoggerFactory.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IMdmEntityServiceFactory factory;

        public MdmService(IMdmEntityServiceFactory factory)
        {
            this.factory = factory;
        }

        public WebResponse<TContract> Create<TContract>(TContract contract, uint version = 0) 
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("MdmService.Create<{0}>", typeof(TContract).Name);
            }
            return this.factory.EntityService<TContract>(version).Create(contract);
        }

        public WebResponse<MdmId> CreateMapping<TContract>(int id, MdmId identifier, uint version = 0) 
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("MdmService.CreateMapping<{0}>: {1} {2}", typeof(TContract).Name, id, identifier);
            }
            return this.factory.EntityService<TContract>(version).CreateMapping(id, identifier);
        }

        public WebResponse<TContract> DeleteMapping<TContract>(int entityId, int mappingId, uint version = 0) where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("MdmService.DeleteMapping<{0}> : {1} {2}", typeof(TContract).Name, entityId, mappingId);
            }
            return this.factory.EntityService<TContract>(version).DeleteMapping(entityId, mappingId);
        }

        public WebResponse<TContract> Get<TContract>(int id, uint version = 0)
            where TContract : IMdmEntity
        {
            return this.Get<TContract>(id, null, version);
        }

        public WebResponse<IList<TContract>> GetList<TContract>(int id, uint version = 0) where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("MdmService.GetList<{0}>: {1}", typeof(TContract).Name, id);
            }
            return this.factory.EntityService<TContract>(version).GetList(id);
        }

        public WebResponse<TContract> Get<TContract>(int id, DateTime? asof, uint version = 0) 
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("MdmService.Get<{0}>: {1} {2}", typeof(TContract).Name, id, asof);
            }
            return this.factory.EntityService<TContract>(version).Get(id, asof);
        }

        public WebResponse<TContract> Get<TContract>(MdmId identifier, uint version = 0)
            where TContract : IMdmEntity
        {
            return this.Get<TContract>(identifier, null, version);
        }

        public WebResponse<TContract> Get<TContract>(MdmId identifier, DateTime? asof, uint version = 0)
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("MdmService.Get<{0}>: {1} {2}", typeof(TContract).Name, identifier, asof);
            }
            return this.factory.EntityService<TContract>(version).Get(identifier);
        }

        public WebResponse<MdmId> GetMapping<TContract>(int id, Predicate<MdmId> query, uint version = 0) 
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("MdmService.GetMapping<{0}> : {1}", typeof(TContract).Name, id);
            }
            return this.factory.EntityService<TContract>(version).GetMapping(id, query);
        }

        public WebResponse<MdmId> Map<TContract>(int id, string targetSystem, uint version = 0) 
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("MdmService.Map<{0}>: {1} {2}", typeof(TContract).Name, id, targetSystem);
            }
            return this.factory.EntityService<TContract>(version).Map(id, targetSystem);
        }

        /// <summary>
        /// Map from a MdmId to another system
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="targetSystem"></param>
        /// <returns></returns>
        public WebResponse<MappingResponse> CrossMap<TContract>(MdmId identifier, string targetSystem, uint version = 0)
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("MdmService.CrossMap<{0}>: {1} {2}", typeof(TContract).Name, identifier, targetSystem);
            }
            return this.factory.EntityService<TContract>(version).CrossMap(identifier, targetSystem);
        }

        /// <summary>
        /// Map from one system to another.
        /// </summary>
        /// <param name="sourceSystem"></param>
        /// <param name="identifier"></param>
        /// <param name="targetSystem"></param>
        /// <returns></returns>
        public WebResponse<MappingResponse> CrossMap<TContract>(string sourceSystem, string identifier, string targetSystem, uint version = 0)
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("MdmService.CrossMap<{0}>: {1} {2} {3}", typeof(TContract).Name, sourceSystem, identifier, targetSystem);
            }
            return this.factory.EntityService<TContract>(version).CrossMap(sourceSystem, identifier, targetSystem); 
        }

        public void Invalidate<TContract>(int id, uint version = 0) 
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("MdmService.Invalidate<{0}> : {1}", typeof(TContract).Name, id);
            }
            this.factory.EntityService<TContract>(version).Invalidate(id);
        }

        public PagedWebResponse<IList<TContract>> Search<TContract>(Search search, uint version = 0)
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("MdmService.Search<{0}>", typeof(TContract).Name);
            }
            return this.factory.EntityService<TContract>(version).Search(search);
        }

        public WebResponse<TContract> Update<TContract>(int id, TContract entity, string etag, uint version = 0) where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("MdmService.Update<{0}>: {1} {2}", typeof(TContract).Name, id, etag);
            }
            return this.factory.EntityService<TContract>(version).Update(id, entity, etag);
        }

        public WebResponse<TContract> Create<TContract>(TContract contract, MdmRequestInfo requestInfo, uint version = 0) where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("MdmService.Create<{0}>", typeof(TContract).Name);
            }
            return this.factory.EntityService<TContract>(version).Create(contract, requestInfo);
        }

        public WebResponse<MdmId> CreateMapping<TContract>(int id, MdmId identifier, MdmRequestInfo requestInfo, uint version = 0) where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("MdmService.CreateMapping<{0}>: {1} {2}", typeof(TContract).Name, id, identifier);
            }
            return this.factory.EntityService<TContract>(version).CreateMapping(id, identifier, requestInfo);
        }

        public WebResponse<TContract> DeleteMapping<TContract>(int entityId, int mappingId, MdmRequestInfo requestInfo, uint version = 0) where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("MdmService.DeleteMapping<{0}> : {1} {2}", typeof(TContract).Name, entityId, mappingId);
            }

            return this.factory.EntityService<TContract>(version).DeleteMapping(entityId, mappingId, requestInfo);
        }

        public WebResponse<TContract> Update<TContract>(int id, TContract contract, string etag, MdmRequestInfo requestInfo, uint version = 0) where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("MdmService.Update<{0}>: {1} {2}", typeof(TContract).Name, id, etag);
            }
            return this.factory.EntityService<TContract>(version).Update(id, contract, etag, requestInfo);
        }
    }
}