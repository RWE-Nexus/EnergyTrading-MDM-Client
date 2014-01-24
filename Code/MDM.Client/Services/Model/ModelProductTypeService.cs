namespace EnergyTrading.MDM.Client.Services.Model
{
    using RWEST.Nexus.MDM.Contracts;

    public class ModelProductTypeService : IMdmModelEntityService<ProductType, Client.Model.ProductType>
    {
        private readonly IMdmModelEntityService service;

        public ModelProductTypeService(IMdmModelEntityService service)
        {
            this.service = service;
        }

        public Client.Model.ProductType Get(RWEST.Nexus.MDM.Contracts.ProductType contract)
        {
            if (contract == null)
            {
                return null;
            }

            var model = new Client.Model.ProductType
            {
                Source = contract,
                Product = this.service.ModelProduct(contract, x => x.Details.Product),
                InstrumentTypeOverrides = this.service.ModelInstrumentTypeOverrides(contract)
            };

            return model;
        }
    }
}