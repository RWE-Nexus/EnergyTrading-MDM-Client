namespace EnergyTrading.Mdm.Client.Services
{
    using EnergyTrading.Mdm.Contracts;

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
        IMdmEntityService<TContract> EntityService<TContract>(uint version = 0)
            where TContract : IMdmEntity;
    }
}