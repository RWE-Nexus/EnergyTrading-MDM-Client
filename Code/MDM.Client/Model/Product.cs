namespace EnergyTrading.MDM.Client.Model
{
    using System.Collections.Generic;
    using System.Linq;

    public class Product : IMdmModelEntity<RWEST.Nexus.MDM.Contracts.Product>
    {
        public Product()
        {
            this.ProductCurves = new List<ProductCurve>();
            this.InstrumentTypeOverrides = new List<InstrumentTypeOverride>();
            this.TenorTypes = new List<TenorType>();
        }

        public RWEST.Nexus.MDM.Contracts.Product Source { get; set; }

        public Model.Exchange Exchange { get; set; }

        public Model.Market Market { get; set; }

        public Model.CommodityInstrumentType CommodityInstrumentType { get; set; }

        public Model.Curve ProductCurve { get; set; }

        public List<Model.ProductCurve> ProductCurves { get; set; }
        
        public List<Model.InstrumentTypeOverride> InstrumentTypeOverrides { get; set; }

        public Model.ProductScota ScotaTerms { get; set; }

        public RWEST.Nexus.MDM.Contracts.Shape Shape { get; set; }

        public List<Model.TenorType> TenorTypes { get; set; }

        public Model.ProductCurve ValuationCurve
        {
            get
            {
                return this.ProductCurves == null 
                     ? null 
                     : this.ProductCurves.FirstOrDefault(p => p.ProductCurveType != null && p.ProductCurveType.ToLower().Equals("valuation"));
            }
        }

        public Model.ProductCurve FinancialCurve
        {
            get
            {
                return this.ProductCurves == null 
                    ? null 
                    : this.ProductCurves.FirstOrDefault(p => p.ProductCurveType != null && p.ProductCurveType.ToLower().Equals("financial"));
            }
        }
    }
}