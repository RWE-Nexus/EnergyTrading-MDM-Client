namespace EnergyTrading.MDM.Client.Tests.Registrars
{
    using EnergyTrading.MDM.Client.Registrars;
    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.WebApiClient;
    using EnergyTrading.MDM.Client.WebClient;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using BrokerRate = RWEST.Nexus.MDM.Contracts.BrokerRate;

    [TestClass]
    public class MdmClientRegistrarFixture
    {
        private IUnityContainer container;

        [TestInitialize]
        public void Setup()
        {
            this.container = new UnityContainer();
            this.container.RegisterInstance(typeof (IServiceLocator), new Mock<IServiceLocator>().Object);

            new MdmClientRegistrar().Register(this.container);
        }

        [TestMethod]
        public void CanResolveHttpClientFactory()
        {
            this.container.Resolve<IHttpClientFactory>();
        }

        [TestMethod]
        public void CanResolveMessageRequester()
        {
            this.container.Resolve<IMessageRequester>();
        }

        [TestMethod]
        public void CanResolveMdmModelEntityService()
        {
            this.container.Resolve<IMdmModelEntityService>();
        }

        [TestMethod]
        public void CanResolveAnExampleMdmModelEntityService()
        {
            this.container.Resolve<IMdmModelEntityService<BrokerRate, Client.Model.BrokerRate>>();
        }
    }
}
