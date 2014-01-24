namespace EnergyTrading.MDM.Client.Tests.Services.Model
{
    using System;

    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.Services.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class ModelCurveServiceFixture
    {
        private Mock<IMdmModelEntityService> mdmService;
        private ModelCurveService service;

        [TestInitialize]
        public void SetUp()
        {
            this.mdmService = new Mock<IMdmModelEntityService>();
            this.service = new ModelCurveService(this.mdmService.Object);
        }

        [TestMethod]
        public void Return_null_if_contract_is_null()
        {
            var curve = this.service.Get(null);

            Assert.IsNull(curve);
        }

        [TestMethod]
        public void Source_is_contract()
        {
            var contract = new Curve();

            var curve = this.service.Get(contract);

            Assert.AreEqual(contract, curve.Source);
        }

        [TestMethod]
        public void Commodity_comes_from_service()
        {
            const int CommodityId = 123;
            var contract = CreateCurve(details => details.Commodity = CommodityId.ToEntityId());
            var commodity = this.mdmService.StubModel<Commodity, EnergyTrading.MDM.Client.Model.Commodity>(CommodityId);

            var curve = this.service.Get(contract);

            Assert.AreEqual(commodity, curve.Commodity);
        }

        [TestMethod]
        public void Location_comes_from_service()
        {
            const int LocationId = 123;
            var contract = CreateCurve(details => details.Location = LocationId.ToEntityId());
            var location = this.mdmService.Stub<Location>(LocationId);

            var curve = this.service.Get(contract);

            Assert.AreEqual(location, curve.Location);
        }

        [TestMethod]
        public void Originator_comes_from_service()
        {
            const int OriginatorId = 123;
            var contract = CreateCurve(details => details.Originator = OriginatorId.ToEntityId());
            var originator = this.mdmService.Stub<Party>(OriginatorId);

            var curve = this.service.Get(contract);

            Assert.AreEqual(originator, curve.Originator);
        }

        private static Curve CreateCurve(Action<CurveDetails> configure = null)
        {
            var contract = new Curve();

            if (configure != null)
            {
                configure(contract.Details);
            }

            return contract;
        }
    }
}