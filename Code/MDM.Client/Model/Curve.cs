namespace EnergyTrading.MDM.Client.Model
{
    public class Curve : IMdmModelEntity<RWEST.Nexus.MDM.Contracts.Curve>
    {
        public RWEST.Nexus.MDM.Contracts.Curve Source { get; set; }

        public Model.Commodity Commodity { get; set; }

        public RWEST.Nexus.MDM.Contracts.Location Location { get; set; }

        public RWEST.Nexus.MDM.Contracts.Party Originator { get; set; }
    }
}
