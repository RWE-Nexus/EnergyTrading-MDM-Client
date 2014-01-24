namespace EnergyTrading.MDM.Client.Services.Model
{
    using RWEST.Nexus.MDM.Contracts;

    public class ModelMarketService : IMdmModelEntityService<Market, Client.Model.Market>
    {
        private readonly IMdmModelEntityService service;

        public ModelMarketService(IMdmModelEntityService service)
        {
            this.service = service;
        }

        public Client.Model.Market Get(RWEST.Nexus.MDM.Contracts.Market contract)
        {
            if (contract == null)
            {
                return null;
            }

            var model = new Client.Model.Market
            {
                Source = contract,
                Calendar = this.service.Calendar(contract.ToMdmKey(x => x.Details.Calendar)),
                Commodity = this.service.ModelCommodity(contract, x => x.Details.Commodity),
                Location = this.service.Location(contract.ToMdmKey(x => x.Details.Location))
            };

            return model;
        }
    }
}