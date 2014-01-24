namespace EnergyTrading.MDM.Client.Services.Model
{
    using RWEST.Nexus.MDM.Contracts;

    public class ModelProductCurveService : IMdmModelEntityService<ProductCurve, Client.Model.ProductCurve>
    {
        private readonly IMdmModelEntityService service;

        public ModelProductCurveService(IMdmModelEntityService service)
        {
            this.service = service;
        }

        public Client.Model.ProductCurve Get(RWEST.Nexus.MDM.Contracts.ProductCurve contract)
        {
            if (contract == null)
            {
                return null;
            }

            var model = new Client.Model.ProductCurve
            {
                Source = contract,
                Name = contract.Details.Name,
                Curve = this.service.ModelCurve(contract, x => x.Details.Curve),
                ProductCurveType = contract.Details.ProductCurveType,
                ProjectionMethod = contract.Details.ProjectionMethod
            };

            return model;
        }
    }
}