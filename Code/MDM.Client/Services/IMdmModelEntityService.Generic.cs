namespace EnergyTrading.MDM.Client.Services
{
    using EnergyTrading.MDM.Client.Model;

    using RWEST.Nexus.MDM.Contracts;

    /// <summary>
    /// Produces model entities from MDM contracts.
    /// </summary>
    /// <typeparam name="TContract"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public interface IMdmModelEntityService<in TContract, out TModel>
        where TContract : class, IMdmEntity
        where TModel : IMdmModelEntity<TContract>
    {
        /// <summary>
        /// Create a model entity given a contract.
        /// </summary>
        /// <param name="contract">Contract to use</param>
        /// <returns>A model entity that wrappers the contract.</returns>
        TModel Get(TContract contract);
    }
}