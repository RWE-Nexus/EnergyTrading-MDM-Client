namespace EnergyTrading.Mdm.Client.Tests.Extensions
{
    using EnergyTrading.Mdm.Client.Extensions;
    using EnergyTrading.Mdm.Contracts;

    using NUnit.Framework;

    [TestFixture]
    public class ServiceUnavailableFixture
    {
        [Test]
        public void ShouldReturnTrueForNotFound()
        {
            var fault = new Fault { Message = "NotFound" };
            Assert.IsTrue(fault.IsServiceUnavailable());
        }

        [Test]
        public void ShouldReturnTrueForServiceUnavailable()
        {
            var fault = new Fault { Message = "ServiceUnavailable" };
            Assert.IsTrue(fault.IsServiceUnavailable());
        }

        [Test]
        public void ShouldReturnTrueForUnableToConnect()
        {
            var fault = new Fault { Message = "Unable to connect to the remote server CA01230" };
            Assert.IsTrue(fault.IsServiceUnavailable());
        }

        [Test]
        public void ShouldReturnFalseForOther()
        {
            var fault = new Fault { Message = "Other errors" };
            Assert.IsFalse(fault.IsServiceUnavailable());
        }
    }
}