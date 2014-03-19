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

        public WebResponse<TContract> Create<TContract>(TContract contract) 
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Create<{0}>", typeof(TContract).Name);
            }
            return this.factory.EntityService<TContract>().Create(contract);
        }

        public WebResponse<MdmId> CreateMapping<TContract>(int id, MdmId identifier) 
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("CreateMapping<{0}>: {1} {2}", typeof(TContract).Name, id, identifier);
            }
            return this.factory.EntityService<TContract>().CreateMapping(id, identifier);
        }

        public WebResponse<TContract> DeleteMapping<TContract>(int entityId, int mappingId) where TContract : IMdmEntity
        {
            return this.factory.EntityService<TContract>().DeleteMapping(entityId, mappingId);
        }

        public WebResponse<TContract> Get<TContract>(int id)
            where TContract : IMdmEntity
        {
            return this.Get<TContract>(id, null);
        }

        public WebResponse<IList<TContract>> GetList<TContract>(int id) where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("GetList<{0}>: {1}", typeof(TContract).Name, id);
            }
            return this.factory.EntityService<TContract>().GetList(id);
        }

        public WebResponse<TContract> Get<TContract>(int id, DateTime? asof) 
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Get<{0}>: {1} {2}", typeof(TContract).Name, id, asof);
            }
            return this.factory.EntityService<TContract>().Get(id, asof);
        }

        public WebResponse<TContract> Get<TContract>(MdmId identifier)
            where TContract : IMdmEntity
        {
            return this.Get<TContract>(identifier, null);
        }

        public WebResponse<TContract> Get<TContract>(MdmId identifier, DateTime? asof)
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Get<{0}>: {1} {2}", typeof(TContract).Name, identifier, asof);
            }
            return this.factory.EntityService<TContract>().Get(identifier);
        }

        public WebResponse<MdmId> GetMapping<TContract>(int id, Predicate<MdmId> query) 
            where TContract : IMdmEntity
        {
            return this.factory.EntityService<TContract>().GetMapping(id, query);
        }

        public WebResponse<MdmId> Map<TContract>(int id, string targetSystem) 
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Map<{0}>: {1} {2}", typeof(TContract).Name, id, targetSystem);
            }
            return this.factory.EntityService<TContract>().Map(id, targetSystem);
        }

        /// <summary>
        /// Map from a MdmId to another system
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="targetSystem"></param>
        /// <returns></returns>
        public WebResponse<MappingResponse> CrossMap<TContract>(MdmId identifier, string targetSystem)
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("CrossMap<{0}>: {1} {2}", typeof(TContract).Name, identifier, targetSystem);
            }
            return this.factory.EntityService<TContract>().CrossMap(identifier, targetSystem);
        }

        /// <summary>
        /// Map from one system to another.
        /// </summary>
        /// <param name="sourceSystem"></param>
        /// <param name="identifier"></param>
        /// <param name="targetSystem"></param>
        /// <returns></returns>
        public WebResponse<MappingResponse> CrossMap<TContract>(string sourceSystem, string identifier, string targetSystem)
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("CrossMap<{0}>: {1} {2} {3}", typeof(TContract).Name, sourceSystem, identifier, targetSystem);
            }
            return this.factory.EntityService<TContract>().CrossMap(sourceSystem, identifier, targetSystem); 
        }

        public void Invalidate<TContract>(int id) 
            where TContract : IMdmEntity
        {
            this.factory.EntityService<TContract>().Invalidate(id);
        }

        public PagedWebResponse<IList<TContract>> Search<TContract>(Search search)
            where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Search<{0}>", typeof(TContract).Name);
            }
            return this.factory.EntityService<TContract>().Search(search);
        }

        public WebResponse<TContract> Update<TContract>(int id, TContract entity, string etag) where TContract : IMdmEntity
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Update<{0}>: {1} {2}", typeof(TContract).Name, id, etag);
            }
            return this.factory.EntityService<TContract>().Update(id, entity, etag);
        }
    }
}