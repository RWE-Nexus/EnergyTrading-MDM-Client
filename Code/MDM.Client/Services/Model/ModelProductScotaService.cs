namespace EnergyTrading.MDM.Client.Services.Model
{
    using RWEST.Nexus.MDM.Contracts;

    public class ModelProductScotaService : IMdmModelEntityService<ProductScota, Client.Model.ProductScota>
    {
        private readonly IMdmModelEntityService service;

        public ModelProductScotaService(IMdmModelEntityService service)
        {
            this.service = service;
        }

        public Client.Model.ProductScota Get(ProductScota contract)
        {
            if (contract == null)
            {
                return null;
            }

            var model = new Client.Model.ProductScota
            {
                Source = contract,
                Rss = contract.Details.ScotaRss,
                Version = contract.Details.ScotaVersion,
                Contract = contract.Details.ScotaContract,
                DeliveryPoint = this.service.Location(contract.ToMdmKey(x => x.Details.ScotaDeliveryPoint)),
                Origin = this.service.Location(contract.ToMdmKey(x => x.Details.ScotaOrigin))
            };

            return model;
        }
    }
}