namespace EnergyTrading.Mdm.Client.Services
{
    using EnergyTrading.Mdm.Contracts;

    using Microsoft.Practices.ServiceLocation;

    public class MdmEntityLocatorFactory : IMdmEntityLocatorService
    {
        private readonly IServiceLocator locator;

        public MdmEntityLocatorFactory(IServiceLocator locator)
        {
            this.locator = locator;
        }

        public TContract Get<TContract>(MdmId id, uint version = 0) 
            where TContract : IMdmEntity
        {
            return this.EntityService<TContract>().Get(id, version);
        }

        private IMdmEntityLocator<TContract> EntityService<TContract>() 
            where TContract : IMdmEntity
        {
            return this.locator.GetInstance<IMdmEntityLocator<TContract>>();
        }
    }
}
