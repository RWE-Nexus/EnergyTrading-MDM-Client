namespace EnergyTrading.MDM.Client.Model
{
    public class CommodityInstrumentType : IMdmModelEntity<RWEST.Nexus.MDM.Contracts.CommodityInstrumentType>
    {
        public RWEST.Nexus.MDM.Contracts.CommodityInstrumentType Source { get; set; }

        public RWEST.Nexus.MDM.Contracts.Commodity Commodity { get; set; }

        public RWEST.Nexus.MDM.Contracts.InstrumentType InstrumentType { get; set; }

        public string InstrumentDelivery { get; set; }

        public string InstrumentSubType { get; set; }
    }
}