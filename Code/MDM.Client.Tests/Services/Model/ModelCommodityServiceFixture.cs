namespace EnergyTrading.MDM.Client.Tests.Services.Model
{
    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.Services.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class ModelCommodityServiceFixture
    {
        private Mock<IMdmModelEntityService> mdmService;
        private ModelCommodityService service;

        [TestInitialize]
        public void SetUp()
        {
            this.mdmService = new Mock<IMdmModelEntityService>();
            this.service = new ModelCommodityService(this.mdmService.Object);
        }

        [TestMethod]
        public void Return_null_if_contract_is_null()
        {
            var commodity = this.service.Get(null);

            Assert.IsNull(commodity);
        }

        [TestMethod]
        public void Source_is_contract()
        {
            var contract = new Commodity();

            var commodity = this.service.Get(contract);

            Assert.AreEqual(contract, commodity.Source);
        }

        [TestMethod]
        public void Parent_comes_from_service()
        {
            const int ParentId = 123;
            var contract = new Commodity { Details = { Parent = ParentId.ToEntityId() } };
            var parentCommodity = this.mdmService.StubModel<Commodity, EnergyTrading.MDM.Client.Model.Commodity>(ParentId);

            var commodity = this.service.Get(contract);

            Assert.AreEqual(parentCommodity, commodity.Parent);
        }
    }
}