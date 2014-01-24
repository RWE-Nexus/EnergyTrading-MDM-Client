namespace MDM.Client.IntegrationTests.Checkers
{
    using RWEST.Nexus.MDM.Contracts;
    using EnergyTrading.Test;

    public class ProductTenorTypeDetailsChecker : Checker<ProductTenorTypeDetails>
    {
        public ProductTenorTypeDetailsChecker()
        {
            Compare(x => x.Product);
            Compare(x => x.TenorType);
        }
    }
}
