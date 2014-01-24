namespace EnergyTrading.MDM.Client.Services
{
    using System;
    using System.Collections.Generic;

    using EnergyTrading.MDM.Client.Extensions;
    using EnergyTrading.MDM.Client.Model;
    using EnergyTrading.MDM.Client.WebClient;

    using EnergyTrading.Contracts.Search;
    using RWEST.Nexus.MDM.Contracts;

    /// <copydocfrom cref="IMdmModelEntityService" />
    public class MdmModelEntityService : IMdmModelEntityService
    {
        private readonly IMdmService mdmService;
        private readonly IMdmModelEntityServiceFactory factory;
        private readonly IMdmEntityLocatorService entityLocatorService;

        /// <summary>
        /// Create a new instance of the <see cref="MdmModelEntityService" /> class.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="mdmService"></param>
        /// <param name="locatorService"></param>
        public MdmModelEntityService(IMdmModelEntityServiceFactory factory, IMdmService mdmService, IMdmEntityLocatorService locatorService)
        {
            this.factory = factory;
            this.mdmService = mdmService;
            this.entityLocatorService = locatorService;
        }

        /// <copydocfrom cref="IMdmModelEntityService.Get{T}(int)" />
        public TContract Get<TContract>(int id) 
            where TContract : IMdmEntity
        {
            if (id == 0)
            {
                return default(TContract);
            }

            Func<WebResponse<TContract>> x = () => this.mdmService.Get<TContract>(id);
            return x.Retry();
        }

        /// <copydocfrom cref="IMdmModelEntityService.Get{T}(NexusId)" />
        public TContract Get<TContract>(NexusId id) 
            where TContract : IMdmEntity
        {
            return this.entityLocatorService.Get<TContract>(id);
        }

        /// <copydocfrom cref="IMdmModelEntityService.Get{T, U}" />
        public TModel Get<TContract, TModel>(TContract contract)
            where TContract : class, IMdmEntity
            where TModel : IMdmModelEntity<TContract>
        {
            return contract == null 
                ? default(TModel) 
                : this.factory.ModelService<TContract, TModel>().Get(contract);
        }

        /// <copydocfrom cref="IMdmModelEntityService.Search{T}" />
        public WebResponse<IList<TContract>> Search<TContract>(Search search)
            where TContract : IMdmEntity
        {
            return this.mdmService.Search<TContract>(search);
        }
    }
}