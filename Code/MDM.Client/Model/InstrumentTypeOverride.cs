namespace EnergyTrading.MDM.Client.Model
{
    public class InstrumentTypeOverride : IMdmModelEntity<RWEST.Nexus.MDM.Contracts.InstrumentTypeOverride>
    {
        public RWEST.Nexus.MDM.Contracts.InstrumentTypeOverride Source { get; set; }

        public string ProductId { get; set; }

        public string ProductTypeId { get; set; }

        public string TenorTypeId { get; set; }

        public string Name { get; set; }

        public Model.Broker Broker { get; set; }

        public Model.CommodityInstrumentType CommodityInstrumentType { get; set; }
    }
}