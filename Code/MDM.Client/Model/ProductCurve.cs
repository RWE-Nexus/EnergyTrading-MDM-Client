namespace EnergyTrading.MDM.Client.Model
{
    public class ProductCurve : IMdmModelEntity<RWEST.Nexus.MDM.Contracts.ProductCurve>
    {
        public RWEST.Nexus.MDM.Contracts.ProductCurve Source { get; set; }

        // NB Can't have product here - would be recursive
        //public Model.Product Product { get; set; }

        public Model.Curve Curve { get; set; }

        public string ProductCurveType { get; set; }

        public string ProjectionMethod { get; set; }

        public string Name { get; set; }
    }
}