namespace EnergyTrading.MDM.Client.Services.Model
{
    using RWEST.Nexus.MDM.Contracts;

    public class ModelProductService : IMdmModelEntityService<Product, Client.Model.Product>
    {
        private readonly IMdmModelEntityService service;

        public ModelProductService(IMdmModelEntityService service)
        {
            this.service = service;
        }

        public Client.Model.Product Get(Product contract)
        {
            if (contract == null)
            {
                return null;
            }

            var model = new Client.Model.Product
            {
                Source = contract,
                Market = this.service.ModelMarket(contract, x => x.Details.Market),
                Exchange = this.service.ModelExchange(contract, x => x.Details.Exchange),
                ProductCurve = this.service.ModelCurve(contract, x => x.Details.DefaultCurve),
                ProductCurves = this.service.ModelProductCurves(contract),
                CommodityInstrumentType = this.service.ModelCommodityInstrumentType(this.service.CommodityInstrumentType(contract.ToMdmKey(x => x.Details.CommodityInstrumentType)), contract.Details.InstrumentSubType),
                InstrumentTypeOverrides = this.service.ModelInstrumentTypeOverrides(contract),
                Shape = this.service.Shape(contract.ToMdmKey(x => x.Details.Shape)),
                ScotaTerms = this.service.ModelProductScota(this.service.ProductScota(contract)),
                TenorTypes = this.service.ModelTenorTypes(this.service.TenorTypes(contract))
            };

            return model;
        }
    }
}