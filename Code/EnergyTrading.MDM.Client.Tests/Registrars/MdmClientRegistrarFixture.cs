namespace EnergyTrading.Mdm.Client.Tests.Registrars
{
    using EnergyTrading.Mdm.Client.Services;
    using EnergyTrading.Mdm.Client.WebApi.Registrars;
    using EnergyTrading.Mdm.Client.WebApi.WebApiClient;
    using EnergyTrading.Mdm.Client.WebClient;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

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
    }
}
