namespace EnergyTrading.MDM.Client.Model
{
    public class BrokerRate : IMdmModelEntity<RWEST.Nexus.MDM.Contracts.BrokerRate>
    {
        public RWEST.Nexus.MDM.Contracts.BrokerRate Source { get; set; }

        public RWEST.Nexus.MDM.Contracts.Broker Broker { get; set; }

        public RWEST.Nexus.MDM.Contracts.PartyRole Desk { get; set; }

        public RWEST.Nexus.MDM.Contracts.CommodityInstrumentType CommodityInstrumentType { get; set; }

        public RWEST.Nexus.MDM.Contracts.Location Location { get; set; }

        public RWEST.Nexus.MDM.Contracts.ProductType ProductType { get; set; }

        public RWEST.Nexus.MDM.Contracts.PartyAction PartyAction { get; set; }

        public decimal Rate { get; set; }

        public string RateType { get; set; }
        
        public string Currency { get; set; }
    }
}