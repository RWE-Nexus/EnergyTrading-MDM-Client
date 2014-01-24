namespace EnergyTrading.MDM.Client.Services
{
    using RWEST.Nexus.MDM.Contracts;

    /// <summary>
    /// Locator a <see cref="IMdmEntity"/> via a <see cref="NexusId" />
    /// </summary>
    public interface IMdmEntityLocator<out TContract>
        where TContract : IMdmEntity
    {
        /// <summary>
        /// Get a MDM entity.
        /// </summary>
        /// <param name="id">NexusId to use</param>
        /// <returns>The MDM entity if found, null otherwise.</returns>
        TContract Get(NexusId id);
    }
}