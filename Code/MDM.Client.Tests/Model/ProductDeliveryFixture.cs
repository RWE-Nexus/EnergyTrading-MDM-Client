namespace EnergyTrading.MDM.Client.Tests.Model
{
    using EnergyTrading.Test;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RWEST.Nexus.MDM.Contracts;

    using ProductDelivery = EnergyTrading.MDM.Client.Model.ProductDelivery;
    using ProductType = EnergyTrading.MDM.Client.Model.ProductType;
    using ProductTypeInstance = EnergyTrading.MDM.Client.Model.ProductTypeInstance;

    [TestClass]
    public class ProductDeliveryFixture : Fixture
    {
        [TestMethod]
        public void DeliveryRangeReturnDefault()
        {
            var value = new ProductDelivery();

            Assert.IsNull(value.DeliveryRange); 
        }

        [TestMethod]
        public void DeliveryRangeReturnAssigned()
        {
            var value = new ProductDelivery { DeliveryRange = "Test" };

            Assert.AreEqual("Test", value.DeliveryRange);
        }

        [TestMethod]
        public void DeliveryRangeReturnPt()
        {
            var ptSource = new RWEST.Nexus.MDM.Contracts.ProductType
            {
                Details = new ProductTypeDetails
                {
                    DeliveryRangeType = "PTI"
                }
            };
            var pt = new ProductType { Source = ptSource };

            var value = new ProductDelivery { ProductType = pt };

            Assert.AreEqual("PTI", value.DeliveryRange);
        }

        [TestMethod]
        public void DeliveryRangeReturnPti()
        {
            var ptSource = new RWEST.Nexus.MDM.Contracts.ProductType
            {
                Details = new ProductTypeDetails
                {
                    DeliveryRangeType = "PT"
                }
            };
            var pt = new ProductType { Source = ptSource };
            var pti = new ProductTypeInstance
            {
                Source = new RWEST.Nexus.MDM.Contracts.ProductTypeInstance(),
                ProductType = pt
            };

            var value = new ProductDelivery { ProductTypeInstance = pti };

            Assert.AreEqual("PT", value.DeliveryRange);
        }
    }
}