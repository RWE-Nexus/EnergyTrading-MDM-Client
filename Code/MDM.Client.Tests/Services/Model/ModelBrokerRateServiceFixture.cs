namespace EnergyTrading.MDM.Client.Tests.Services.Model
{
    using System;

    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.Services.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class ModelBrokerRateServiceFixture
    {
        private Mock<IMdmModelEntityService> mdmService;
        private ModelBrokerRateService service;

        [TestInitialize]
        public void SetUp()
        {
            this.mdmService = new Mock<IMdmModelEntityService>();
            this.service = new ModelBrokerRateService(this.mdmService.Object);
        }

        [TestMethod]
        public void Return_null_if_contract_is_null()
        {
            var brokerRate = this.service.Get(null);

            Assert.IsNull(brokerRate);
        }

        [TestMethod]
        public void Broker_rate_source_is_contract()
        {
            var contract = new BrokerRate();

            var brokerRate = this.service.Get(contract);

            Assert.AreEqual(contract, brokerRate.Source);
        }

        [TestMethod]
        public void Broker_comes_from_service()
        {
            const int BrokerId = 123;
            var contract = CreateBrokerRate(details => details.Broker = BrokerId.ToEntityId());
            var broker = this.mdmService.Stub<Broker>(BrokerId);

            var brokerRate = this.service.Get(contract);

            Assert.AreEqual(broker, brokerRate.Broker);
        }

        [TestMethod]
        public void Desk_comes_from_service()
        {
            const int DeskId = 123;
            var contract = CreateBrokerRate(details => details.Desk = DeskId.ToEntityId());
            var desk = this.mdmService.Stub<PartyRole>(DeskId);

            var brokerRate = this.service.Get(contract);

            Assert.AreEqual(desk, brokerRate.Desk);
        }

        [TestMethod]
        public void Commodity_instrument_type_comes_from_service()
        {
            const int CommodityInstrumentTypeId = 123;
            var contract = CreateBrokerRate(details => details.CommodityInstrumentType = CommodityInstrumentTypeId.ToEntityId());
            var commodityInstrumentType = this.mdmService.Stub<CommodityInstrumentType>(CommodityInstrumentTypeId);

            var brokerRate = this.service.Get(contract);

            Assert.AreEqual(commodityInstrumentType, brokerRate.CommodityInstrumentType);
        }

        [TestMethod]
        public void Location_comes_from_service()
        {
            const int LocationId = 123;
            var contract = CreateBrokerRate(details => details.Location = LocationId.ToEntityId());
            var location = this.mdmService.Stub<Location>(LocationId);

            var brokerRate = this.service.Get(contract);

            Assert.AreEqual(location, brokerRate.Location);
        }

        [TestMethod]
        public void Product_type_comes_from_service()
        {
            const int ProductTypeId = 123;
            var contract = CreateBrokerRate(details => details.ProductType = ProductTypeId.ToEntityId());
            var productType = this.mdmService.Stub<ProductType>(ProductTypeId);

            var brokerRate = this.service.Get(contract);

            Assert.AreEqual(productType, brokerRate.ProductType);
        }

        [TestMethod]
        public void Party_action_comes_from_contract_details()
        {
            const PartyAction PartyAction = PartyAction.Aggressor;
            var contract = CreateBrokerRate(details => details.PartyAction = PartyAction);

            var brokerRate = this.service.Get(contract);

            Assert.AreEqual(PartyAction, brokerRate.PartyAction);
        }

        [TestMethod]
        public void Rate_comes_from_contract_details()
        {
            const decimal Rate = 15m;
            var contract = CreateBrokerRate(details => details.Rate = Rate);

            var brokerRate = this.service.Get(contract);

            Assert.AreEqual(Rate, brokerRate.Rate);
        }

        [TestMethod]
        public void RateType_comes_from_contract_details()
        {
            const string RateType = "per unit";
            var contract = CreateBrokerRate(details => details.RateType = RateType);

            var brokerRate = this.service.Get(contract);

            Assert.AreEqual(RateType, brokerRate.RateType);
        }

        [TestMethod]
        public void Currency_comes_from_contract_details()
        {
            const string Currency = "GBP";
            var contract = CreateBrokerRate(details => details.Currency = Currency);

            var brokerRate = this.service.Get(contract);

            Assert.AreEqual(Currency, brokerRate.Currency);
        }

        private static BrokerRate CreateBrokerRate(Action<BrokerRateDetails> configure = null)
        {
            var contract = new BrokerRate();

            if (configure != null)
            {
                configure(contract.Details);
            }

            return contract;
        }
    }
}