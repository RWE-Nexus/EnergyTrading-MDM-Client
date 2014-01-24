namespace EnergyTrading.MDM.Client.Tests.Services.Model
{
    using System;
    using System.Globalization;

    using EnergyTrading.MDM.Client.Model;
    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.Services.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using RWEST.Nexus.MDM.Contracts;

    using ProductType = EnergyTrading.MDM.Client.Model.ProductType;
    using ProductTypeInstance = RWEST.Nexus.MDM.Contracts.ProductTypeInstance;

    [TestClass]
    public class ModelProductTypeInstanceServiceFixture
    {
        private Mock<IMdmModelEntityService> mdmService;
        private ModelProductTypeInstanceService service;

        [TestInitialize]
        public void SetUp()
        {
            this.mdmService = new Mock<IMdmModelEntityService>();
            this.service = new ModelProductTypeInstanceService(this.mdmService.Object);
        }

        [TestMethod]
        public void Return_null_if_contract_is_null()
        {
            var productTypeInstance = this.service.Get(null);

            Assert.IsNull(productTypeInstance);
        }

        [TestMethod]
        public void Source_is_contract()
        {
            var contract = CreateProductTypeInstance();

            var productTypeInstance = this.service.Get(contract);

            Assert.AreEqual(contract, productTypeInstance.Source);
        }

        [TestMethod]
        public void Product_type_comes_from_service()
        {
            const int ProductTypeId = 123;
            var contract = CreateProductTypeInstance(details => details.ProductType = CreateEntityId(ProductTypeId));
            var productType = this.StubModel<RWEST.Nexus.MDM.Contracts.ProductType, ProductType>(ProductTypeId);

            var product = this.service.Get(contract);

            Assert.AreEqual(productType, product.ProductType);
        }

        private static ProductTypeInstance CreateProductTypeInstance(Action<ProductTypeInstanceDetails> configure = null)
        {
            var contract = new ProductTypeInstance();

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

        private TModel StubModel<TContract, TModel>(int id)
            where TContract : class, IMdmEntity, new()
            where TModel : IMdmModelEntity<TContract>, new()
        {
            var contract = this.Stub<TContract>(id);
            var model = new TModel();
            this.mdmService.Setup(s => s.Get<TContract, TModel>(contract)).Returns(model);
            return model;
        }
    }
}