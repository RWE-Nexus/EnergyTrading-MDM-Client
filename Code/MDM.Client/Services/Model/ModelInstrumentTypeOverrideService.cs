namespace EnergyTrading.MDM.Client.Services.Model
{
    using RWEST.Nexus.MDM.Contracts;

    public class ModelInstrumentTypeOverrideService : IMdmModelEntityService<InstrumentTypeOverride, Client.Model.InstrumentTypeOverride>
    {
        private readonly IMdmModelEntityService service;

        public ModelInstrumentTypeOverrideService(IMdmModelEntityService service)
        {
            this.service = service;
        }

        public Client.Model.InstrumentTypeOverride Get(InstrumentTypeOverride contract)
        {
            if (contract == null)
            {
                return null;
            }

            // ProductType may be not be found as ProductType is not compulsory - we may have ProductTenorType instead.
            // Preserved here for legacy interaction with ModelProductForStandingData
            var productType = this.service.ProductType(contract.ToMdmKey(x => x.Details.ProductType));

            var productTenorType = this.service.ProductTenorType(contract.ToMdmKey(x => x.Details.ProductTenorType));

            var model = new Client.Model.InstrumentTypeOverride
            {
                Source = contract,
                Name = contract.Details.Name,
                ProductId = productType == null ? null : productType.Details.Product.Identifier.Identifier,
                ProductTypeId = productType == null ? null : productType.Identifiers.PrimaryIdentifier().Identifier,
                TenorTypeId = productTenorType == null ? null : productTenorType.Details.TenorType.Identifier.Identifier,
                Broker = this.service.ModelBroker(contract, x => x.Details.Broker),
                CommodityInstrumentType = this.service.ModelCommodityInstrumentType(contract, x => x.Details.CommodityInstrumentType, contract.Details.InstrumentSubType)
            };

            return model;
        }
    }
}