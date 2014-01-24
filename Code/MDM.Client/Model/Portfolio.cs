namespace EnergyTrading.MDM.Client.Model
{
    public class Portfolio : IMdmModelEntity<RWEST.Nexus.MDM.Contracts.Portfolio>
    {
        public RWEST.Nexus.MDM.Contracts.Portfolio Source { get; set; }

        public RWEST.Nexus.MDM.Contracts.PartyRole BusinessUnit { get; set; }
    }
}