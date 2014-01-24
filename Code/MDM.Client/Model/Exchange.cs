namespace EnergyTrading.MDM.Client.Model
{
    /// <summary>
    /// Expanded version of a <see cref="RWEST.Nexus.MDM.Contracts.Exchange" /> entity.
    /// </summary>
    public class Exchange : IMdmModelEntity<RWEST.Nexus.MDM.Contracts.Exchange>
    {
        /// <summary>
        /// Gets or sets the source entity.
        /// </summary>
        public RWEST.Nexus.MDM.Contracts.Exchange Source { get; set; }

        public string Name { get; set; }
    }
}