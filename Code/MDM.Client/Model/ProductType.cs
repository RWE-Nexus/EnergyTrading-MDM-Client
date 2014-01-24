namespace EnergyTrading.MDM.Client.Model
{
    using System.Collections.Generic;

    public class ProductType : IMdmModelEntity<RWEST.Nexus.MDM.Contracts.ProductType>
    {
        public ProductType()
        {
            this.InstrumentTypeOverrides = new List<InstrumentTypeOverride>();
        }

        public RWEST.Nexus.MDM.Contracts.ProductType Source { get; set; }

        public Model.Product Product { get; set; }

        public List<Model.InstrumentTypeOverride> InstrumentTypeOverrides { get; set; }
    }
}