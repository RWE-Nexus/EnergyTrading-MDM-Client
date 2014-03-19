namespace EnergyTrading.Mdm.Client.Services
{
    using System;
    using System.Collections.Concurrent;

    using Microsoft.Practices.ServiceLocation;

    public class LocatorMdmEntityServiceFactory : IMdmEntityServiceFactory
    {
        private readonly IServiceLocator locator;
        private readonly ConcurrentDictionary<Type, object> cache;

        public LocatorMdmEntityServiceFactory(IServiceLocator locator)
        {
            this.locator = locator;
            this.cache = new ConcurrentDictionary<Type, object>();
        }

        public IMdmEntityService<TContract> EntityService<TContract>() where TContract : EnergyTrading.Mdm.Contracts.IMdmEntity
        {
            var type = typeof(TContract);
            object value;
            if (!this.cache.TryGetValue(type, out value))
            {
                value = this.locator.GetInstance<IMdmEntityService<TContract>>();
                this.cache.AddOrUpdate(type, value, (x, y) => value);
            }

            return (IMdmEntityService<TContract>)value;
        }
    }
}
