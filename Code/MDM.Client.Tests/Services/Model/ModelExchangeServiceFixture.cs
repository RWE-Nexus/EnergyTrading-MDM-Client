namespace EnergyTrading.MDM.Client.Tests.Services.Model
{
    using EnergyTrading.MDM.Client.Services.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class ModelExchangeServiceFixture
    {
        private ModelExchangeService service;

        [TestInitialize]
        public void SetUp()
        {
            this.service = new ModelExchangeService();
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
            var contract = new Exchange();

            var curve = this.service.Get(contract);

            Assert.AreEqual(contract, curve.Source);
        }

        [TestMethod]
        public void Name_comes_from_contract_details()
        {
            const string Name = "name";
            var contract = new Exchange
            {
                Details =
                {
                    Name = Name
                }
            };

            var exchange = this.service.Get(contract);

            Assert.AreEqual(Name, exchange.Name);
        }
    }
}