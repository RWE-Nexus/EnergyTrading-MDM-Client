namespace EnergyTrading.MDM.Client.Services.Model
{
    using RWEST.Nexus.MDM.Contracts;

    public class ModelCommodityInstrumentTypeService : IMdmModelEntityService<CommodityInstrumentType, Client.Model.CommodityInstrumentType>
    {
        private readonly IMdmModelEntityService service;

        public ModelCommodityInstrumentTypeService(IMdmModelEntityService service)
        {
            this.service = service;
        }

        public Client.Model.CommodityInstrumentType Get(RWEST.Nexus.MDM.Contracts.CommodityInstrumentType contract)
        {
            if (contract == null)
            {
                return null;
            }

            var model = new Client.Model.CommodityInstrumentType
            {
                Source = contract,
                Commodity = this.service.Commodity(contract.ToMdmKey(x => x.Details.Commodity)),
                InstrumentType = this.service.InstrumentType(contract.ToMdmKey(x => x.Details.InstrumentType)),
                InstrumentDelivery = contract.Details.InstrumentDelivery,
                // NOTE: Populated via extension method
                //InstrumentSubType = instrumentSubType
            };

            return model;
        }
    }
}