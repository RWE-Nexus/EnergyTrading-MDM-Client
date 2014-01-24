namespace EnergyTrading.MDM.Client.Tests.Extensions
{
    using System.Collections.Generic;

    using EnergyTrading.MDM.Client.Extensions;
    using EnergyTrading.MDM.Client.WebClient;
    using EnergyTrading.Test;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class HandleResponseTransformFixture : Fixture
    {
        [TestMethod]
        public void ShouldReturnEmptyListOnNull()
        {
            WebResponse<IList<Broker>> response = null;

            var result = response.HandleResponse(Transform);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ShouldReturnEmptyListIfInvalid()
        {
            var response = new WebResponse<IList<Broker>>
            {
                IsValid = false,
                Message = new List<Broker> { new Broker() }
            };

            var result = response.HandleResponse(Transform);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ShouldTransformItemsInResponseIfValid()
        {
            var response = new WebResponse<IList<Broker>>
            {
                IsValid = true,
                Message = new List<Broker> { new Broker(), new Broker() }
            };

            var result = response.HandleResponse(Transform);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        private static int Transform(Broker broker)
        {
            return 1;
        }
    }
}