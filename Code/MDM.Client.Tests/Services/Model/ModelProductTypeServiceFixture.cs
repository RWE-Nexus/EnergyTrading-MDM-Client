namespace EnergyTrading.MDM.Client.Tests.Services.Model
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using EnergyTrading.MDM.Client.Model;
    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.Services.Model;
    using EnergyTrading.MDM.Client.WebClient;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using RWEST.Nexus.MDM.Contracts;

    using InstrumentTypeOverride = RWEST.Nexus.MDM.Contracts.InstrumentTypeOverride;
    using Product = EnergyTrading.MDM.Client.Model.Product;
    using ProductType = RWEST.Nexus.MDM.Contracts.ProductType;

    [TestClass]
    public class ModelProductTypeServiceFixture
    {
        private Mock<IMdmModelEntityService> mdmService;
        private ModelProductTypeService service;

        [TestInitialize]
        public void SetUp()
        {
            this.mdmService = new Mock<IMdmModelEntityService>();
            this.service = new ModelProductTypeService(this.mdmService.Object);
        }

        [TestMethod]
        public void Return_null_if_contract_is_null()
        {
            var productType = this.service.Get(null);

            Assert.IsNull(productType);
        }

        [TestMethod]
        public void Source_is_contract()
        {
            var contract = CreateProductType();
            this.StubInstrumentTypeOverrideSearch();

            var productTypeInstance = this.service.Get(contract);

            Assert.AreEqual(contract, productTypeInstance.Source);
        }

        [TestMethod]
        public void Product_comes_from_service()
        {
            const int ProductId = 123;
            var contract = CreateProductType(details => details.Product = CreateEntityId(ProductId));
            this.StubInstrumentTypeOverrideSearch();
            var product = this.StubModel<RWEST.Nexus.MDM.Contracts.Product, Product>(ProductId);

            var productType = this.service.Get(contract);

            Assert.AreEqual(product, productType.Product);
        }

        [TestMethod]
        public void Instrument_type_overrides_come_from_service()
        {
            var contract = CreateProductType();
            var instrumentTypeOverride = new InstrumentTypeOverride();
            this.StubInstrumentTypeOverrideSearch(instrumentTypeOverride);
            var model = this.StubModel<InstrumentTypeOverride, EnergyTrading.MDM.Client.Model.InstrumentTypeOverride>(instrumentTypeOverride);

            var productType = this.service.Get(contract);

            Assert.AreEqual(1, productType.InstrumentTypeOverrides.Count);
            Assert.AreEqual(model, productType.InstrumentTypeOverrides.First());
        }

        private static ProductType CreateProductType(Action<ProductTypeDetails> configure = null)
        {
            var contract = new ProductType();

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

        private TModel StubModel<TContract, TModel>(TContract contract)
            where TContract : class, IMdmEntity, new()
            where TModel : IMdmModelEntity<TContract>, new()
        {
            var model = new TModel();
            this.mdmService.Setup(s => s.Get<TContract, TModel>(contract)).Returns(model);
            return model;
        }

        private void StubInstrumentTypeOverrideSearch(InstrumentTypeOverride instrumentTypeOverride = null)
        {
            this.mdmService.Setup(s => s.Search<InstrumentTypeOverride>(It.IsAny<EnergyTrading.Contracts.Search.Search>()))
                .Returns(new WebResponse<IList<InstrumentTypeOverride>>
                {
                    Message = new[] { instrumentTypeOverride }
                });
        }
    }
}