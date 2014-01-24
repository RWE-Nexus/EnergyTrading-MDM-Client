namespace EnergyTrading.MDM.Client.Services.Model
{
    using RWEST.Nexus.MDM.Contracts;

    public class ModelCommodityService : IMdmModelEntityService<Commodity, Client.Model.Commodity>
    {
        private readonly IMdmModelEntityService service;

        public ModelCommodityService(IMdmModelEntityService service)
        {
            this.service = service;
        }

        public Client.Model.Commodity Get(RWEST.Nexus.MDM.Contracts.Commodity contract)
        {
            if (contract == null)
            {
                return null;
            }

            var model = new Client.Model.Commodity
            {
                Source = contract,
                Parent = this.service.ModelCommodity(contract, x => x.Details.Parent)
            };

            return model;
        }
    }
}