namespace EnergyTrading.Mdm.Client.Services
{
    using EnergyTrading.Mdm.Client.Extensions;
    using EnergyTrading.Mdm.Contracts;

    /// <summary>
    /// Uses an <see cref="IMdmEntityService{T}" /> for entity location.
    /// </summary>
    public class MdmServiceMdmEntityLocator<TContract> : IMdmEntityLocator<TContract>
        where TContract : class, IMdmEntity
    {
        /// <summary>
        /// Create a new instance of the <see cref="MdmServiceMdmEntityLocator{T}"/> class.
        /// </summary>
        /// <param name="service"></param>
        public MdmServiceMdmEntityLocator(IMdmEntityService<TContract> service)
        {
            this.Service = service;
        }

        /// <summary>
        /// Gets the Service property.
        /// </summary>
        protected IMdmEntityService<TContract> Service { get; private set; }

        /// <copydocfrom cref="IMdmEntityLocator{T}.Get" />
        public TContract Get(MdmId id)
        {
            return WebResponseUtility.Retry(() => this.Service.Get(id));
        }
    }
}