namespace EnergyTrading.Mdm.Client.Services
{
    using System.Collections.Generic;

    using EnergyTrading.Contracts.Search;
    using EnergyTrading.Mdm.Client.Model;
    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;

    /// <summary>
    /// Locates MDM contracts and model entities.
    /// </summary>
    public interface IMdmModelEntityService : IMdmEntityLocatorService
    {
        /// <summary>
        /// Get a MDM contract.
        /// </summary>
        /// <typeparam name="TContract">Type of contract to find</typeparam>
        /// <param name="id">Identifier to use</param>
        /// <param name="version">contract version default = 0</param>
        /// <returns>MDM contract if found, null otherwise</returns>
        TContract Get<TContract>(int id, uint version = 0)
            where TContract : IMdmEntity;

        /// <summary>
        /// Get a MDM model based on its contract.
        /// </summary>
        /// <typeparam name="TContract">Type of contract to find</typeparam>
        /// <typeparam name="TModel">Type of model to find</typeparam>        
        /// <param name="contract">Contract to use</param>
        /// <param name="version">contract version default = 0</param>
        /// <returns>MDM contract if found, null otherwise</returns>
        TModel Get<TContract, TModel>(TContract contract, uint version = 0)
            where TContract : class, IMdmEntity
            where TModel : IMdmModelEntity<TContract>;

        /// <summary>
        /// Find MDM contracts.
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="search"></param>
        /// <param name="version">contract version default = 0</param>
        /// <returns></returns>
        WebResponse<IList<TContract>> Search<TContract>(Search search, uint version = 0)
            where TContract : IMdmEntity;
    }
}