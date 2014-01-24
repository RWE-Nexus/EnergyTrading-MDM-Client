namespace EnergyTrading.MDM.Client.Model
{
    public class Market : IMdmModelEntity<RWEST.Nexus.MDM.Contracts.Market>
    {
        public RWEST.Nexus.MDM.Contracts.Market Source { get; set; }

        public Model.Commodity Commodity { get; set; }

        public RWEST.Nexus.MDM.Contracts.Location Location { get; set; }

        public RWEST.Nexus.MDM.Contracts.Calendar Calendar { get; set; }
    }
}