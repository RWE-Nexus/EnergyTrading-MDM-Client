namespace EnergyTrading.MDM.Client.Services.Model
{
    using RWEST.Nexus.MDM.Contracts;

    public class ModelBrokerRateService : IMdmModelEntityService<BrokerRate, Client.Model.BrokerRate>
    {
        private readonly IMdmModelEntityService service;

        public ModelBrokerRateService(IMdmModelEntityService service)
        {
            this.service = service;
        }

        public Client.Model.BrokerRate Get(RWEST.Nexus.MDM.Contracts.BrokerRate contract)
        {
            if (contract == null)
            {
                return null;
            }

            var model = new Client.Model.BrokerRate
            {
                Source = contract,
                Broker = this.service.Broker(contract.ToMdmKey(x => x.Details.Broker)),
                Desk = this.service.PartyRole(contract, x => x.Details.Desk),
                CommodityInstrumentType = this.service.CommodityInstrumentType(contract.ToMdmKey(x => x.Details.CommodityInstrumentType)),
                Location = this.service.Location(contract.ToMdmKey(x => x.Details.Location)),
                ProductType = this.service.ProductType(contract.ToMdmKey(x => x.Details.ProductType)),
                PartyAction = contract.Details.PartyAction,
                Rate = contract.Details.Rate,
                RateType = contract.Details.RateType,
                Currency = contract.Details.Currency
            };

            return model;
        }
    }
}