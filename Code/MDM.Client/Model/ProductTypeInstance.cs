namespace EnergyTrading.MDM.Client.Model
{
    public class ProductTypeInstance : IMdmModelEntity<RWEST.Nexus.MDM.Contracts.ProductTypeInstance>
    {
        public RWEST.Nexus.MDM.Contracts.ProductTypeInstance Source { get; set; }

        public Model.ProductType ProductType { get; set; }
    }
}