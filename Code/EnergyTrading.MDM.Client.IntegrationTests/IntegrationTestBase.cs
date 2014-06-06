namespace EnergyTrading.Mdm.Client.IntegrationTests
{
    using EnergyTrading.Container.Unity;
    using EnergyTrading.Logging;
    using EnergyTrading.Mdm.Client.WebApi.Registrars;
    using EnergyTrading.Test;

    using Microsoft.Practices.ServiceLocation;

    public abstract class IntegrationTestBase : Fixture
    {
        protected override ICheckerFactory CreateCheckerFactory()
        {
            return new CheckerFactory();
        }

        protected override void OnSetup()
        {
            var lm = new SimpleLoggerFactory(new NullLogger());
            LoggerFactory.SetProvider(() => lm);

            var locator = Container.Resolve<IServiceLocator>();
            Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(() => locator);

            new MdmClientRegistrar().Register(Container);
        }
    }
}