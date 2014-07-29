namespace EnergyTrading.Mdm.Client.Services
{
    using System;
    using System.Collections.Generic;

    using EnergyTrading.Contracts.Search;
    using EnergyTrading.Mdm.Client.Extensions;
    using EnergyTrading.Mdm.Client.Model;
    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;

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
        public TContract Get<TContract>(int id, uint version = 0) 
            where TContract : IMdmEntity
        {
            if (id == 0)
            {
                return default(TContract);
            }

            Func<WebResponse<TContract>> x = () => this.mdmService.Get<TContract>(id, version);
            return x.Retry();
        }

        /// <copydocfrom cref="IMdmModelEntityService.Get{T}(MdmId)" />
        public TContract Get<TContract>(MdmId id, uint version = 0) 
            where TContract : IMdmEntity
        {
            return this.entityLocatorService.Get<TContract>(id, version);
        }

        /// <copydocfrom cref="IMdmModelEntityService.Get{T, U}" />
        public TModel Get<TContract, TModel>(TContract contract, uint version = 0)
            where TContract : class, IMdmEntity
            where TModel : IMdmModelEntity<TContract>
        {
            return contract == null 
                ? default(TModel) 
                : this.factory.ModelService<TContract, TModel>(version).Get(contract);
        }

        /// <copydocfrom cref="IMdmModelEntityService.Search{T}" />
        public WebResponse<IList<TContract>> Search<TContract>(Search search, uint version = 0)
            where TContract : IMdmEntity
        {
            return this.mdmService.Search<TContract>(search, version);
        }
    }
}