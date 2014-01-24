namespace EnergyTrading.MDM.Client.Tests.Services.Model
{
    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.Services.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class ModelMarketServiceFixture
    {
        private Mock<IMdmModelEntityService> mdmService;
        private ModelMarketService service;

        [TestInitialize]
        public void SetUp()
        {
            this.mdmService = new Mock<IMdmModelEntityService>();
            this.service = new ModelMarketService(this.mdmService.Object);
        }

        [TestMethod]
        public void Return_null_if_contract_is_null()
        {
            var market = this.service.Get(null);

            Assert.IsNull(market);
        }

        [TestMethod]
        public void Source_is_contract()
        {
            var contract = new Market();

            var market = this.service.Get(contract);

            Assert.AreEqual(contract, market.Source);
        }

        [TestMethod]
        public void Calendar_comes_from_service()
        {
            const int CalendarId = 123;
            var contract = new Market
            {
                Details = new MarketDetails
                {
                    Calendar = CalendarId.ToEntityId(),
                }
            };
            var calendar = this.mdmService.Stub<RWEST.Nexus.MDM.Contracts.Calendar>(CalendarId);

            var market = this.service.Get(contract);

            Assert.AreEqual(calendar, market.Calendar);
        }

        [TestMethod]
        public void Commodity_comes_from_service()
        {
            const int CommodityId = 123;
            var contract = new Market
            {
                Details = new MarketDetails
                {
                    Commodity = CommodityId.ToEntityId(),
                }
            };
            var commodity = this.mdmService.StubModel<Commodity, EnergyTrading.MDM.Client.Model.Commodity>(CommodityId);

            var market = this.service.Get(contract);

            Assert.AreEqual(commodity, market.Commodity);
        }

        [TestMethod]
        public void Location_comes_from_service()
        {
            const int LocationId = 123;
            var contract = new Market
            {
                Details = new MarketDetails
                {
                    Location = LocationId.ToEntityId(),
                }
            };
            var location = this.mdmService.Stub<Location>(LocationId);

            var market = this.service.Get(contract);

            Assert.AreEqual(location, market.Location);
        }
    }
}