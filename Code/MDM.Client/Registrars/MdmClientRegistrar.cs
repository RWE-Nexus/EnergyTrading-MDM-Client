namespace EnergyTrading.MDM.Client.Registrars
{
    using System.Configuration;
    using System.Linq;

    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.WebApiClient;
    using EnergyTrading.MDM.Client.WebClient;

    using Microsoft.Practices.Unity;

    using EnergyTrading.Caching;
    using EnergyTrading.Container.Unity;
    using RWEST.Nexus.MDM.Contracts;

    public class MdmClientRegistrar : IContainerRegistrar
    {
        public int MdmCacheTimeout
        {
            get
            {
                int cacheTimeout;
                return int.TryParse(ConfigurationManager.AppSettings["MdmCacheTimeout"], out cacheTimeout)
                           ? cacheTimeout
                           : 120;
            }
        }

        public bool MdmCaching
        {
            get
            {
                bool cache;
                return bool.TryParse(ConfigurationManager.AppSettings["MdmCaching"], out cache) && cache;
            }
        }

        public string BaseUri
        {
            get { return ConfigurationManager.AppSettings["MdmUri"]; }
        }

        public void Register(IUnityContainer container)
        {
            // Http clients
            container.RegisterType<IHttpClientFactory, HttpClientFactory>();

            // Requester
            container.RegisterType<IMessageRequester, MessageRequester>();

            // MDM Entity Services
            this.RegisterMdmService<Agreement>(container, "agreement");
            this.RegisterMdmService<Book>(container, "book");
            this.RegisterMdmService<Broker>(container, "broker");
            this.RegisterMdmService<BrokerCommodity>(container, "brokercommodity");
            this.RegisterMdmService<BrokerRate>(container, "brokerrate");
            this.RegisterMdmService<BusinessUnit>(container, "businessUnit");
            this.RegisterMdmService<Calendar>(container, "calendar");
            this.RegisterMdmService<Commodity>(container, "commodity");
            this.RegisterMdmService<CommodityInstrumentType>(container, "commodityinstrumenttype");
            this.RegisterMdmService<Counterparty>(container, "counterparty");
            this.RegisterMdmService<Curve>(container, "curve");
            this.RegisterMdmService<BookDefault>(container, "bookdefault");
            this.RegisterMdmService<Dimension>(container, "dimension");
            this.RegisterMdmService<Exchange>(container, "exchange");
            this.RegisterMdmService<Hierarchy>(container, "hierarchy");
            this.RegisterMdmService<InstrumentType>(container, "instrumenttype");
            this.RegisterMdmService<InstrumentTypeOverride>(container, "instrumenttypeoverride");
            this.RegisterMdmService<LegalEntity>(container, "legalentity");
            this.RegisterMdmService<Location>(container, "location");
            this.RegisterMdmService<Market>(container, "market");
            this.RegisterMdmService<Party>(container, "party");
            this.RegisterMdmService<PartyAccountability>(container, "partyaccountability");
            this.RegisterMdmService<PartyCommodity>(container, "partycommodity");
            this.RegisterMdmService<PartyOverride>(container, "partyoverride");
            this.RegisterMdmService<PartyRole>(container, "partyrole");
            this.RegisterMdmService<PartyRoleAccountability>(container, "partyroleaccountability");
            this.RegisterMdmService<Person>(container, "person");
            this.RegisterMdmService<Portfolio>(container, "portfolio");
            this.RegisterMdmService<Product>(container, "product");
            this.RegisterMdmService<ProductType>(container, "producttype");
            this.RegisterMdmService<ProductTenorType>(container, "producttenortype");
            this.RegisterMdmService<ProductTypeInstance>(container, "producttypeinstance");
            this.RegisterMdmService<ProductCurve>(container, "productcurve");
            this.RegisterMdmService<ProductScota>(container, "productscota");
            this.RegisterMdmService<SettlementContact>(container, "settlementcontact");
            this.RegisterMdmService<Shape>(container, "shape");
            this.RegisterMdmService<ShapeDay>(container, "shapeday");
            this.RegisterMdmService<ShapeElement>(container, "shapeelement");
            this.RegisterMdmService<ShipperCode>(container, "shippercode");
            this.RegisterMdmService<SourceSystem>(container, "sourcesystem");
            this.RegisterMdmService<Tenor>(container, "tenor");
            this.RegisterMdmService<TenorType>(container, "tenortype");
            this.RegisterMdmService<Unit>(container, "unit");
            this.RegisterMdmService<Vessel>(container, "vessel");
            //Register new MDM entity CommodityFeeType
            this.RegisterMdmService<CommodityFeeType>(container, "CommodityFeeType");
            this.RegisterMdmService<FeeType>(container, "FeeType");
            this.RegisterReferenceDataService(container);

            // MDM Client service
            container.RegisterType<IMdmEntityServiceFactory, LocatorMdmEntityServiceFactory>();
            container.RegisterType<IMdmService, MdmService>();

            // Register the IMdmEntityLocators factory
            container.RegisterType<IMdmEntityLocatorService, MdmEntityLocatorFactory>();

            // Standard locator, no chain
            container.RegisterType(typeof(IMdmEntityLocator<>), typeof(MdmServiceMdmEntityLocator<>));

            this.RegisterModelEntityServices(container);
        }

        private void RegisterModelEntityServices(IUnityContainer container)
        {
            container.RegisterType<IMdmModelEntityService, MdmModelEntityService>();
            container.RegisterType<IMdmModelEntityServiceFactory, LocatorMdmModelEntityServiceFactory>();

            foreach (var type in this.GetType().Assembly.GetTypes())
            {
                var @interface = type.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof (IMdmModelEntityService<,>)));

                if (@interface != null)
                {
                    container.RegisterType(@interface, type);
                }
            }
        }

        private void RegisterMdmService<T>(IUnityContainer container, string url)
            where T : class, IMdmEntity
        {
            if (this.MdmCaching)
            {
                var cachekey = "Mdm." + typeof(T).Name;

                container.RegisterAbsoluteCacheItemPolicyFactory(cachekey);

                container.RegisterType<IMdmEntityService<T>, MdmEntityService<T>>(
                    url,
                    new InjectionConstructor(
                        this.BaseUri + "/" + url,
                        new ResolvedParameter<IMessageRequester>()));

                // Singleton as we have a cache
                container.RegisterType<IMdmEntityService<T>, CachePolicyMdmEntityService<T>>(
                    new ContainerControlledLifetimeManager(),
                    new InjectionConstructor(
                        new ResolvedParameter<IMdmEntityService<T>>(url),
                        new ResolvedParameter<ICacheItemPolicyFactory>(cachekey)));
            }
            else
            {
                container.RegisterType<IMdmEntityService<T>, MdmEntityService<T>>(
                    new InjectionConstructor(this.BaseUri + "/" + url, new ResolvedParameter<IMessageRequester>()));
            }
        }

        private void RegisterReferenceDataService(IUnityContainer container)
        {
            container.RegisterType<IReferenceDataService, ReferenceDataService>(
                new InjectionConstructor(this.BaseUri + "/referencedata", new ResolvedParameter<IMessageRequester>()));
        }
    }
}