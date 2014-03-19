namespace EnergyTrading.Mdm.Client.Tests.Extensions
{
    using System.Collections.Generic;

    using EnergyTrading.Mdm.Client.Extensions;
    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;
    using EnergyTrading.Test;

    using NUnit.Framework;

    [TestFixture]
    public class HandleResponseFixture : Fixture
    {
        [Test]
        public void ShouldReturnEmptyListOnNull()
        {
            WebResponse<IList<SourceSystem>> response = null;

            var result = response.HandleResponse();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ShouldReturnEmptyListIfInvalid()
        {
            var response = new WebResponse<IList<SourceSystem>>
            {
                IsValid = false,
                Message = new List<SourceSystem> { new SourceSystem() }
            };

            var result = response.HandleResponse();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ShouldReturnItemsInResponseIfValid()
        {
            var response = new WebResponse<IList<SourceSystem>>
            {
                IsValid = true,
                Message = new List<SourceSystem> { new SourceSystem(), new SourceSystem() }
            };

            var result = response.HandleResponse();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }
    }
}