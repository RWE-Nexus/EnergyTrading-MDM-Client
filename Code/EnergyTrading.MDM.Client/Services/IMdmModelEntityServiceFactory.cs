namespace EnergyTrading.Mdm.Client.Services
{
    using EnergyTrading.Mdm.Client.Model;
    using EnergyTrading.Mdm.Contracts;

    /// <summary>
    /// Factory for producing <see cref="IMdmModelEntityService{T, U}" /> instances.
    /// </summary>
    public interface IMdmModelEntityServiceFactory
    {
        /// <summary>
        /// Get a MDM model service.
        /// </summary>
        /// <typeparam name="TContract">Type of the MDM contract</typeparam>
        /// <typeparam name="TModel">Type of the MDM model.</typeparam>
        /// <returns>A <see cref="IMdmModelEntityService{T, U}" /> class.</returns>
        IMdmModelEntityService<TContract, TModel> ModelService<TContract, TModel>()
            where TContract : class, IMdmEntity
            where TModel : IMdmModelEntity<TContract>;
    }
}