namespace EnergyTrading.MDM.Client.Model
{
    public class Commodity : IMdmModelEntity<RWEST.Nexus.MDM.Contracts.Commodity>
    {
        public RWEST.Nexus.MDM.Contracts.Commodity Source { get; set; }

        public Commodity Parent { get; set; }
    }
}
