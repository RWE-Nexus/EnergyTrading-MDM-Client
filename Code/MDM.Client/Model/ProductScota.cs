namespace EnergyTrading.MDM.Client.Model
{
    public class ProductScota : IMdmModelEntity<RWEST.Nexus.MDM.Contracts.ProductScota>
    {
        public RWEST.Nexus.MDM.Contracts.ProductScota Source { get; set; }

        // TODO: Do we need this - wholly owned by Product
        public Model.Product Product { get; set; }

        public RWEST.Nexus.MDM.Contracts.Location DeliveryPoint { get; set; }

        public RWEST.Nexus.MDM.Contracts.Location Origin { get; set; }

        public string Rss { get; set; }

        public string Contract { get; set; }

        public string Version { get; set; }
    }
}