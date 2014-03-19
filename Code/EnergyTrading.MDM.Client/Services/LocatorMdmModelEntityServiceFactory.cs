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
        private readonly ConcurrentDictionary<Tuple<Type, Type>, object> cache;

        /// <summary>
        /// Creates a new instance of the <see cref="LocatorMdmModelEntityServiceFactory" />
        /// </summary>
        /// <param name="locator">Service locator to use.</param>
        public LocatorMdmModelEntityServiceFactory(IServiceLocator locator)
        {
            this.locator = locator;
            this.cache = new ConcurrentDictionary<Tuple<Type, Type>, object>();
        }

        /// <copydocfrom cref="IMdmModelEntityServiceFactory.ModelService{T, U}" />
        public IMdmModelEntityService<TContract, TModel> ModelService<TContract, TModel>()
            where TContract : class, IMdmEntity
            where TModel : IMdmModelEntity<TContract>
        {
            var type = new Tuple<Type, Type>(typeof(TContract), typeof(TModel));
            object value;
            if (!this.cache.TryGetValue(type, out value))
            {
                value = this.locator.GetInstance<IMdmModelEntityService<TContract, TModel>>();
                this.cache.AddOrUpdate(type, value, (x, y) => value);
            }

            return (IMdmModelEntityService<TContract, TModel>) value;
        }
    }
}