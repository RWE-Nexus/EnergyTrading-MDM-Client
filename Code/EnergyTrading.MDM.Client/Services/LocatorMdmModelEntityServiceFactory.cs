namespace EnergyTrading.Mdm.Client.Services
{
    using System;
    using System.Collections.Concurrent;

    using EnergyTrading.Mdm.Client.Model;
    using EnergyTrading.Mdm.Contracts;

    using Microsoft.Practices.ServiceLocation;

    /// <copydocfrom cref="IMdmModelEntityServiceFactory" />
    /// <remarks>Uses a <see cref="IServiceLocator" /> to find the underlying services.</remarks>
    public class LocatorMdmModelEntityServiceFactory : IMdmModelEntityServiceFactory
    {
        private readonly IServiceLocator locator;
        private readonly ConcurrentDictionary<Tuple<Type, Type, string>, object> cache;

        /// <summary>
        /// Creates a new instance of the <see cref="LocatorMdmModelEntityServiceFactory" />
        /// </summary>
        /// <param name="locator">Service locator to use.</param>
        public LocatorMdmModelEntityServiceFactory(IServiceLocator locator)
        {
            this.locator = locator;
            this.cache = new ConcurrentDictionary<Tuple<Type, Type, string>, object>();
        }

        /// <copydocfrom cref="IMdmModelEntityServiceFactory.ModelService{T, U}" />
        public IMdmModelEntityService<TContract, TModel> ModelService<TContract, TModel>(uint version = 0)
            where TContract : class, IMdmEntity
            where TModel : IMdmModelEntity<TContract>
        {
            var type = new Tuple<Type, Type, string>(typeof(TContract), typeof(TModel), version == 0 ? string.Empty : "V" + version);
            object value;
            if (!this.cache.TryGetValue(type, out value))
            {
                value = version == 0 ? this.locator.GetInstance<IMdmModelEntityService<TContract, TModel>>()
                    : this.locator.GetInstance<IMdmModelEntityService<TContract, TModel>>("V" + version);
                this.cache.AddOrUpdate(type, value, (x, y) => value);
            }

            return (IMdmModelEntityService<TContract, TModel>) value;
        }
    }
}