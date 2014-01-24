namespace EnergyTrading.MDM.Client.Model
{
    public class ShipperCode : IMdmModelEntity<RWEST.Nexus.MDM.Contracts.ShipperCode>
    {
        public RWEST.Nexus.MDM.Contracts.ShipperCode Source { get; set; }

        public RWEST.Nexus.MDM.Contracts.Location Location { get; set; }

        public RWEST.Nexus.MDM.Contracts.Party Party { get; set; }
    }
}
