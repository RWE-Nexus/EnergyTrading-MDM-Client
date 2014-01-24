namespace EnergyTrading.MDM.Client.Services
{
    using RWEST.Nexus.MDM.Contracts;

    /// <summary>
    /// Factory for producing <see cref="IMdmEntityService{T}" /> instances.
    /// </summary>
    public interface IMdmEntityServiceFactory
    {
        /// <summary>
        /// Get a MDM entity service
        /// </summary>
        /// <typeparam name="TContract">Type of MDM contract to use</typeparam>
        /// <returns>A <see cref="IMdmEntityService{T}" /> class.</returns>
        IMdmEntityService<TContract> EntityService<TContract>()
            where TContract : IMdmEntity;
    }
}