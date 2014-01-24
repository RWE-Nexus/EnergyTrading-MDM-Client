namespace EnergyTrading.MDM.Client.Tests.Services.Model
{
    using System;

    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.Services.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class ModelCommodityInstrumentTypeServiceFixture
    {
        private Mock<IMdmModelEntityService> mdmService;
        private ModelCommodityInstrumentTypeService service;

        [TestInitialize]
        public void SetUp()
        {
            this.mdmService = new Mock<IMdmModelEntityService>();
            this.service = new ModelCommodityInstrumentTypeService(this.mdmService.Object);
        }

        [TestMethod]
        public void Return_null_if_contract_is_null()
        {
            var commodityInstrumentType = this.service.Get(null);

            Assert.IsNull(commodityInstrumentType);
        }

        [TestMethod]
        public void Source_is_contract()
        {
            var contract = new CommodityInstrumentType();

            var commodityInstrumentType = this.service.Get(contract);

            Assert.AreEqual(contract, commodityInstrumentType.Source);
        }

        [TestMethod]
        public void Commodity_comes_from_service()
        {
            const int CommodityId = 123;
            var contract = CreateCommodityInstrumentType(details => details.Commodity = CommodityId.ToEntityId());
            var commodity = this.mdmService.Stub<Commodity>(CommodityId);

            var commodityInstrumentType = this.service.Get(contract);

            Assert.AreEqual(commodity, commodityInstrumentType.Commodity);
        }

        [TestMethod]
        public void Instrument_type_comes_from_service()
        {
            const int InstrumentTypeId = 123;
            var contract = CreateCommodityInstrumentType(details => details.InstrumentType = InstrumentTypeId.ToEntityId());
            var instrumentType = this.mdmService.Stub<InstrumentType>(InstrumentTypeId);

            var commodityInstrumentType = this.service.Get(contract);

            Assert.AreEqual(instrumentType, commodityInstrumentType.InstrumentType);
        }

        [TestMethod]
        public void Instrument_delivery_comes_from_contract_details()
        {
            const string InstrumentDelivery = "instrumentDelivery";
            var contract = CreateCommodityInstrumentType(details => details.InstrumentDelivery = InstrumentDelivery);

            var commodityInstrumentType = this.service.Get(contract);

            Assert.AreEqual(InstrumentDelivery, commodityInstrumentType.InstrumentDelivery);
        }

        private static CommodityInstrumentType CreateCommodityInstrumentType(Action<CommodityInstrumentTypeDetails> configure = null)
        {
            var contract = new CommodityInstrumentType();

            if (configure != null)
            {
                configure(contract.Details);
            }

            return contract;
        }
    }
}