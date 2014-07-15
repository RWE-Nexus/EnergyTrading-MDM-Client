namespace EnergyTrading.Mdm.Client.Services
{
    using EnergyTrading.Mdm.Client.Extensions;
    using EnergyTrading.Mdm.Contracts;

    /// <summary>
    /// Uses an <see cref="IMdmEntityServiceFactory" /> for entity location.
    /// </summary>
    public class MdmEntityServiceFactoryMdmEntityLocator<TContract> : IMdmEntityLocator<TContract>
        where TContract : class, IMdmEntity
    {
        /// <summary>
        /// Create a new instance of the <see cref="MdmServiceMdmEntityLocator{T}"/> class.
        /// </summary>
        /// <param name="service"></param>
        public MdmEntityServiceFactoryMdmEntityLocator(IMdmEntityServiceFactory factory)
        {
            this.Factory = factory;
        }

        /// <summary>
        /// Gets the Service property.
        /// </summary>
        protected IMdmEntityServiceFactory Factory { get; private set; }

        /// <copydocfrom cref="IMdmEntityLocator{T}.Get" />
        public TContract Get(MdmId id, uint version = 0)
        {
            return WebResponseUtility.Retry(() => this.Factory.EntityService<TContract>(version).Get(id));
        }
    }
}