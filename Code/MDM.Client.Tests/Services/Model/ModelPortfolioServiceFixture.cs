namespace EnergyTrading.MDM.Client.Tests.Services.Model
{
    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.Services.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class ModelPortfolioServiceFixture
    {
        private Mock<IMdmModelEntityService> mdmService;
        private ModelPortfolioService service;

        [TestInitialize]
        public void SetUp()
        {
            this.mdmService = new Mock<IMdmModelEntityService>();
            this.service = new ModelPortfolioService(this.mdmService.Object);
        }

        [TestMethod]
        public void Return_null_if_contract_is_null()
        {
            var portfolio = this.service.Get(null);

            Assert.IsNull(portfolio);
        }

        [TestMethod]
        public void Source_is_contract()
        {
            var contract = new Portfolio();

            var portfolio = this.service.Get(contract);

            Assert.AreEqual(contract, portfolio.Source);
        }

        [TestMethod]
        public void Business_unit_comes_from_service()
        {
            const int BusinessUnitId = 123;
            var contract = new Portfolio
            {
                Details = new PortfolioDetails
                {
                    BusinessUnit = BusinessUnitId.ToEntityId(),
                }
            };
            var businessUnit = this.mdmService.Stub<PartyRole>(BusinessUnitId);

            var portfolio = this.service.Get(contract);

            Assert.AreEqual(businessUnit, portfolio.BusinessUnit);
        }
    }
}