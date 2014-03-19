namespace EnergyTrading.Mdm.Client.WebApi.Registrars
{
    using System.Configuration;
    using System.Linq;

    using EnergyTrading.Caching;
    using EnergyTrading.Container.Unity;
    using EnergyTrading.Mdm.Client.Services;
    using EnergyTrading.Mdm.Client.WebApi.WebApiClient;
    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;

    using Microsoft.Practices.Unity;

    public class MdmClientRegistrar : IContainerRegistrar
    {
        public virtual int MdmCacheTimeout
        {
            get
            {
                int cacheTimeout;
                return int.TryParse(ConfigurationManager.AppSettings["MdmCacheTimeout"], out cacheTimeout)
                           ? cacheTimeout
                           : 120;
            }
        }

        public virtual bool MdmCaching
        {
            get
            {
                bool cache;
                return bool.TryParse(ConfigurationManager.AppSettings["MdmCaching"], out cache) && cache;
            }
        }

        public virtual string BaseUri
        {
            get { return ConfigurationManager.AppSettings["MdmUri"]; }
        }

        public virtual void Register(IUnityContainer container)
        {
            // Http clients
            container.RegisterType<IHttpClientFactory, HttpClientFactory>();

            // Requester
            container.RegisterType<IMessageRequester, MessageRequester>();

            // MDM Entity Services
            this.RegisterMdmService<SourceSystem>(container, "sourcesystem");
            this.RegisterEntityServices(container);
            this.RegisterReferenceDataService(container);

            // MDM Client service
            container.RegisterType<IMdmEntityServiceFactory, LocatorMdmEntityServiceFactory>();
            container.RegisterType<IMdmService, MdmService>();

            // Register the IMdmEntityLocators factory
            container.RegisterType<IMdmEntityLocatorService, MdmEntityLocatorFactory>();

            // Standard locator, no chain
            container.RegisterType(typeof(IMdmEntityLocator<>), typeof(MdmServiceMdmEntityLocator<>));

            container.RegisterType<IMdmModelEntityService, MdmModelEntityService>();
            container.RegisterType<IMdmModelEntityServiceFactory, LocatorMdmModelEntityServiceFactory>();
            this.RegisterModelEntityServices(container);
        }

        protected virtual void RegisterEntityServices(IUnityContainer container)
        {
        }

        protected virtual void RegisterModelEntityServices(IUnityContainer container)
        {
        }

        protected virtual void RegisterMdmService<T>(IUnityContainer container, string url)
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

        protected virtual void RegisterReferenceDataService(IUnityContainer container)
        {
            container.RegisterType<IReferenceDataService, ReferenceDataService>(
                new InjectionConstructor(this.BaseUri + "/referencedata", new ResolvedParameter<IMessageRequester>()));
        }
    }
}