namespace EnergyTrading.MDM.Client.Tests.Services.Model
{
    using System;

    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.Services.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class ModelProductScotaServiceFixture
    {
        private Mock<IMdmModelEntityService> mdmService;
        private ModelProductScotaService service;

        [TestInitialize]
        public void SetUp()
        {
            this.mdmService = new Mock<IMdmModelEntityService>();
            this.service = new ModelProductScotaService(this.mdmService.Object);
        }

        [TestMethod]
        public void Return_null_if_contract_is_null()
        {
            var productScota = this.service.Get(null);

            Assert.IsNull(productScota);
        }

        [TestMethod]
        public void Source_is_contract()
        {
            var contract = new ProductScota();

            var productScota = this.service.Get(contract);

            Assert.AreEqual(contract, productScota.Source);
        }

        [TestMethod]
        public void Rss_comes_from_contract_details()
        {
            const string Rss = "rss";
            var contract = CreateProductScota(details => details.ScotaRss = Rss);

            var productScota = this.service.Get(contract);

            Assert.AreEqual(Rss, productScota.Rss);
        }

        [TestMethod]
        public void Version_comes_from_contract_details()
        {
            const string Version = "version";
            var contract = CreateProductScota(details => details.ScotaVersion = Version);

            var productScota = this.service.Get(contract);

            Assert.AreEqual(Version, productScota.Version);
        }

        [TestMethod]
        public void Contract_comes_from_contract_details()
        {
            const string Contract = "contract";
            var contract = CreateProductScota(details => details.ScotaContract = Contract);

            var productScota = this.service.Get(contract);

            Assert.AreEqual(Contract, productScota.Contract);
        }

        [TestMethod]
        public void Delivery_point_comes_from_service()
        {
            const int DeliveryPointId = 123;
            var contract = CreateProductScota(details => details.ScotaDeliveryPoint = DeliveryPointId.ToEntityId());
            var deliveryPoint = this.mdmService.Stub<Location>(DeliveryPointId);

            var productScota = this.service.Get(contract);

            Assert.AreEqual(deliveryPoint, productScota.DeliveryPoint);
        }

        [TestMethod]
        public void Origin_comes_from_service()
        {
            const int OriginId = 123;
            var contract = CreateProductScota(details => details.ScotaOrigin = OriginId.ToEntityId());
            var origin = this.mdmService.Stub<Location>(OriginId);

            var productScota = this.service.Get(contract);

            Assert.AreEqual(origin, productScota.Origin);
        }

        private static ProductScota CreateProductScota(Action<ProductScotaDetails> configure)
        {
            var contract = new ProductScota();
            configure(contract.Details);
            return contract;
        }
    }
}