namespace EnergyTrading.MDM.Client.Services
{
    using RWEST.Nexus.MDM.Contracts;

    /// <summary>
    /// Finds MDM contracts based on an identifier
    /// </summary>
    public interface IMdmEntityLocatorService
    {
        /// <summary>
        /// Retrieve an MDM contract based on a key.
        /// </summary>
        /// <typeparam name="TContract">Type of contract to find</typeparam>
        /// <param name="id">Identifier to use</param>
        /// <returns>MDM contract if found, null otherwise</returns>
        TContract Get<TContract>(NexusId id) 
            where TContract : IMdmEntity;
    }
}