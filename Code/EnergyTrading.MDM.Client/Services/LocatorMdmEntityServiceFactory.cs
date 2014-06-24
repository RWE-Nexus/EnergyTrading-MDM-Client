namespace EnergyTrading.Mdm.Client.Services
{
    using System;
    using System.Collections.Concurrent;

    using Microsoft.Practices.ServiceLocation;

    public class LocatorMdmEntityServiceFactory : IMdmEntityServiceFactory
    {
        private readonly IServiceLocator locator;
        private readonly ConcurrentDictionary<string, object> cache;

        public LocatorMdmEntityServiceFactory(IServiceLocator locator)
        {
            this.locator = locator;
            this.cache = new ConcurrentDictionary<string, object>();
        }

        private string CreateCacheKey(Type type, uint version)
        {
            return version.ToString() + type.ToString();
        }

        public IMdmEntityService<TContract> EntityService<TContract>(uint version = 0) where TContract : EnergyTrading.Mdm.Contracts.IMdmEntity
        {
            var key = this.CreateCacheKey(typeof(TContract), version);
            object value;
            if (!this.cache.TryGetValue(key, out value))
            {
                value = version == 0 ? this.locator.GetInstance<IMdmEntityService<TContract>>() : this.locator.GetInstance<IMdmEntityService<TContract>>("V" + version);
                this.cache.AddOrUpdate(key, value, (x, y) => value);
            }

            return (IMdmEntityService<TContract>)value;
        }
    }
}
