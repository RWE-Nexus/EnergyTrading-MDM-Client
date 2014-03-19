using EnergyTrading.Container.Unity;

using Microsoft.Practices.ServiceLocation;

namespace MDM.Client.IntegrationTests
{
    using EnergyTrading.Logging;
    using EnergyTrading.Mdm.Client.WebApi.Registrars;
    using EnergyTrading.Test;

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