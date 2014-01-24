namespace EnergyTrading.MDM.Client.Tests.Services.Model
{
    using EnergyTrading.MDM.Client.Services.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class ModelBrokerServiceFixture
    {
        private ModelBrokerService modelBrokerService;

        [TestInitialize]
        public void SetUp()
        {
            this.modelBrokerService = new ModelBrokerService();
        }

        [TestMethod]
        public void Return_null_if_contract_is_null()
        {
            var brokerRate = this.modelBrokerService.Get(null);

            Assert.IsNull(brokerRate);
        }

        [TestMethod]
        public void Broker_source_is_contract()
        {
            var contract = new Broker();

            var brokerRate = this.modelBrokerService.Get(contract);

            Assert.AreEqual(contract, brokerRate.Source);
        }

        [TestMethod]
        public void Name_comes_from_contract_details()
        {
            const string Name = "broker";
            var contract = new Broker
            {
                Details = new BrokerDetails { Name = Name }
            };

            var broker = this.modelBrokerService.Get(contract);

            Assert.AreEqual(Name, broker.Name);
        }
    }
}