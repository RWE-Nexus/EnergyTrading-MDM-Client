namespace EnergyTrading.Mdm.Client.Services
{
    using EnergyTrading.Mdm.Contracts;

    /// <summary>
    /// Locator a <see cref="IMdmEntity"/> via a <see cref="MdmId" />
    /// </summary>
    public interface IMdmEntityLocator<out TContract>
        where TContract : IMdmEntity
    {
        /// <summary>
        /// Get a MDM entity.
        /// </summary>
        /// <param name="id">MdmId to use</param>
        /// <returns>The MDM entity if found, null otherwise.</returns>
        TContract Get(MdmId id);
    }
}