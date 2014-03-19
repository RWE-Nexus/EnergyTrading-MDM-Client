namespace EnergyTrading.Mdm.Client.Tests.Extensions
{
    using EnergyTrading.Mdm.Client.Extensions;
    using EnergyTrading.Mdm.Contracts;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ServiceUnavailableFixture
    {
        [TestMethod]
        public void ShouldReturnTrueForNotFound()
        {
            var fault = new Fault { Message = "NotFound" };
            Assert.IsTrue(fault.IsServiceUnavailable());
        }

        [TestMethod]
        public void ShouldReturnTrueForServiceUnavailable()
        {
            var fault = new Fault { Message = "ServiceUnavailable" };
            Assert.IsTrue(fault.IsServiceUnavailable());
        }

        [TestMethod]
        public void ShouldReturnTrueForUnableToConnect()
        {
            var fault = new Fault { Message = "Unable to connect to the remote server CA01230" };
            Assert.IsTrue(fault.IsServiceUnavailable());
        }

        [TestMethod]
        public void ShouldReturnFalseForOther()
        {
            var fault = new Fault { Message = "Other errors" };
            Assert.IsFalse(fault.IsServiceUnavailable());
        }
    }
}