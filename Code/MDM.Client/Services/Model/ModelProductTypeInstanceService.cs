namespace EnergyTrading.MDM.Client.Services.Model
{
    using RWEST.Nexus.MDM.Contracts;

    public class ModelProductTypeInstanceService : IMdmModelEntityService<ProductTypeInstance, Client.Model.ProductTypeInstance>
    {
        private readonly IMdmModelEntityService service;

        public ModelProductTypeInstanceService(IMdmModelEntityService service)
        {
            this.service = service;
        }

        public Client.Model.ProductTypeInstance Get(RWEST.Nexus.MDM.Contracts.ProductTypeInstance contract)
        {
            if (contract == null)
            {
                return null;
            }

            var model = new Client.Model.ProductTypeInstance
            {
                Source = contract,
                ProductType = this.service.ModelProductType(contract, x => x.Details.ProductType)
            };

            return model;
        }
    }
}