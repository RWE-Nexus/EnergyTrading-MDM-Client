namespace EnergyTrading.MDM.Client.Model
{
    /// <summary>
    /// Level of product information acquired.
    /// </summary>
    public class ProductHierarchyLevel
    {
        /// <summary>
        /// We don't know the level.
        /// </summary>
        public const string Unknown = "Unknown";

        /// <summary>
        /// Have identified a product, e.g. NBP
        /// </summary>
        public const string Product = "Product";

        /// <summary>
        /// Have identified a product type, e.g. NBP Months
        /// </summary>
        public const string ProductType = "ProductType";

        /// <summary>
        /// Have identified a product type instance e.g. NBP Dec 13
        /// </summary>
        public const string ProductTypeInstance = "ProductTypeInstance";
    }
}
