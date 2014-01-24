using Microsoft.Practices.ServiceLocation;
using EnergyTrading.Container.Unity;

namespace MDM.Client.IntegrationTests
{
    using EnergyTrading.MDM.Client.Registrars;

    using EnergyTrading.Logging;
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

            new MdmClientRegistrarWebApi().Register(Container);
        }
    }
}