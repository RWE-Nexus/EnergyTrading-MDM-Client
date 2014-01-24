namespace EnergyTrading.MDM.Client.Services
{
    using Microsoft.Practices.ServiceLocation;

    using RWEST.Nexus.MDM.Contracts;

    public class MdmEntityLocatorFactory : IMdmEntityLocatorService
    {
        private readonly IServiceLocator locator;

        public MdmEntityLocatorFactory(IServiceLocator locator)
        {
            this.locator = locator;
        }

        public TContract Get<TContract>(NexusId id) 
            where TContract : IMdmEntity
        {
            return this.EntityService<TContract>().Get(id);
        }

        private IMdmEntityLocator<TContract> EntityService<TContract>() 
            where TContract : IMdmEntity
        {
            return this.locator.GetInstance<IMdmEntityLocator<TContract>>();
        }
    }
}
