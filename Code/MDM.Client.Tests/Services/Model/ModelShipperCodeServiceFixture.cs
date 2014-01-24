namespace EnergyTrading.MDM.Client.Tests.Services.Model
{
    using System;
    using System.Globalization;

    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.Services.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class ModelShipperCodeServiceFixture
    {
        private Mock<IMdmModelEntityService> mdmService;
        private ModelShipperCodeService service;

        [TestInitialize]
        public void SetUp()
        {
            this.mdmService = new Mock<IMdmModelEntityService>();
            this.service = new ModelShipperCodeService(this.mdmService.Object);
        }

        [TestMethod]
        public void Return_null_if_contract_is_null()
        {
            var shipperCode = this.service.Get(null);

            Assert.IsNull(shipperCode);
        }

        [TestMethod]
        public void Source_is_contract()
        {
            var contract = CreateContract();

            var shipperCode = this.service.Get(contract);

            Assert.AreEqual(contract, shipperCode.Source);
        }

        [TestMethod]
        public void Location_comes_from_service()
        {
            const int LocationId = 123;
            var contract = CreateContract(details => details.Location = CreateEntityId(LocationId));
            var location = this.Stub<Location>(LocationId);

            var shipperCode = this.service.Get(contract);

            Assert.AreEqual(location, shipperCode.Location);
        }

        [TestMethod]
        public void Party_comes_from_service()
        {
            const int PartyId = 123;
            var contract = CreateContract(details => details.Party = CreateEntityId(PartyId));
            var party = this.Stub<Party>(PartyId);

            var shipperCode = this.service.Get(contract);

            Assert.AreEqual(party, shipperCode.Party);
        }

        private static ShipperCode CreateContract(Action<ShipperCodeDetails> configure = null)
        {
            var contract = new ShipperCode();

            if (configure != null)
            {
                configure(contract.Details);
            }

            return contract;
        }

        private static EntityId CreateEntityId(int id)
        {
            return new EntityId { Identifier = new NexusId { Identifier = id.ToString(CultureInfo.InvariantCulture) } };
        }

        private TContract Stub<TContract>(int id)
            where TContract : class, IMdmEntity, new()
        {
            var contract = new TContract();
            this.mdmService.Setup(s => s.Get<TContract>(id)).Returns(contract);
            return contract;
        }
    }
}