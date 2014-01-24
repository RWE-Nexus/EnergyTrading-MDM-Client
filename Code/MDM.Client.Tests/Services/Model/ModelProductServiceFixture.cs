namespace EnergyTrading.MDM.Client.Tests.Services.Model
{
    using System;
    using System.Linq;

    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.Services.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using RWEST.Nexus.Contracts.Atom;
    using RWEST.Nexus.MDM.Contracts;

    using Market = EnergyTrading.MDM.Client.Model.Market;

    [TestClass]
    public class ModelProductServiceFixture
    {
        private Mock<IMdmModelEntityService> mdmService;
        private ModelProductService service;

        [TestInitialize]
        public void SetUp()
        {
            this.mdmService = new Mock<IMdmModelEntityService>();
            this.service = new ModelProductService(this.mdmService.Object);
        }

        [TestMethod]
        public void Return_null_if_contract_is_null()
        {
            var product = this.service.Get(null);

            Assert.IsNull(product);
        }

        [TestMethod]
        public void Source_is_contract()
        {
            var contract = CreateProduct();
            this.mdmService.StubSearch<ProductScota>();
            this.mdmService.StubSearch<ProductTenorType>();

            var product = this.service.Get(contract);

            Assert.AreEqual(contract, product.Source);
        }

        [TestMethod]
        public void Market_comes_from_service()
        {
            const int MarketId = 123;
            var contract = CreateProduct(details => details.Market = MarketId.ToEntityId());
            var market = this.mdmService.StubModel<RWEST.Nexus.MDM.Contracts.Market, Market>(MarketId);
            this.mdmService.StubSearch<ProductScota>();
            this.mdmService.StubSearch<ProductTenorType>();

            var product = this.service.Get(contract);

            Assert.AreEqual(market, product.Market);
        }

        [TestMethod]
        public void Exchange_comes_from_service()
        {
            const int ExchangeId = 123;
            var contract = CreateProduct(details => details.Exchange = ExchangeId.ToEntityId());
            var exchange = this.mdmService.StubModel<Exchange, EnergyTrading.MDM.Client.Model.Exchange>(ExchangeId);
            this.mdmService.StubSearch<ProductScota>();
            this.mdmService.StubSearch<ProductTenorType>();

            var product = this.service.Get(contract);

            Assert.AreEqual(exchange, product.Exchange);
        }

        [TestMethod]
        public void Product_curve_comes_from_service()
        {
            const int ProductCurveId = 123;
            var contract = CreateProduct(details => details.DefaultCurve = ProductCurveId.ToEntityId());
            var curve = this.mdmService.StubModel<Curve, EnergyTrading.MDM.Client.Model.Curve>(ProductCurveId);
            this.mdmService.StubSearch<ProductScota>();
            this.mdmService.StubSearch<ProductTenorType>();

            var product = this.service.Get(contract);

            Assert.AreEqual(curve, product.ProductCurve);
        }

        [TestMethod]
        public void Product_curves_come_from_service()
        {
            const int ProductCurveId = 123;
            var contract = CreateProduct();
            var type = typeof(ProductCurve).Name;
            contract.Links.Add(new Link
            {
                Type = type,
                Uri = string.Format("/{0}/{1}", type, ProductCurveId)
            });
            var productCurve = this.mdmService.StubModel<ProductCurve, EnergyTrading.MDM.Client.Model.ProductCurve>(ProductCurveId);
            this.mdmService.StubSearch<ProductScota>();
            this.mdmService.StubSearch<ProductTenorType>();

            var product = this.service.Get(contract);

            Assert.AreEqual(1, product.ProductCurves.Count);
            Assert.AreEqual(productCurve, product.ProductCurves.First());
        }

        [TestMethod]
        public void Commodity_instrument_type_comes_from_service()
        {
            const int CommodityInstrumentTypeId = 123;
            var contract = CreateProduct(details => details.CommodityInstrumentType = CommodityInstrumentTypeId.ToEntityId());
            var commodityInstrumentType = this.mdmService.StubModel<CommodityInstrumentType, EnergyTrading.MDM.Client.Model.CommodityInstrumentType>(CommodityInstrumentTypeId);
            this.mdmService.StubSearch<ProductScota>();
            this.mdmService.StubSearch<ProductTenorType>();

            var product = this.service.Get(contract);

            Assert.AreEqual(commodityInstrumentType, product.CommodityInstrumentType);
        }

        [TestMethod]
        public void Shape_comes_from_service()
        {
            const int ShapeId = 123;
            var contract = CreateProduct(details => details.Shape = ShapeId.ToEntityId());
            var shape = this.mdmService.Stub<Shape>(ShapeId);
            this.mdmService.StubSearch<ProductScota>();
            this.mdmService.StubSearch<ProductTenorType>();

            var product = this.service.Get(contract);

            Assert.AreEqual(shape, product.Shape);
        }

        [TestMethod]
        public void Scota_terms_come_from_service()
        {
            var contract = CreateProduct();
            var productScota = new ProductScota();
            this.mdmService.StubSearch(productScota);
            var modelProductScota = this.mdmService.StubModel<ProductScota, EnergyTrading.MDM.Client.Model.ProductScota>(productScota);
            this.mdmService.StubSearch<ProductTenorType>();

            var product = this.service.Get(contract);

            Assert.AreEqual(modelProductScota, product.ScotaTerms);
        }

        [TestMethod]
        public void Tenor_types_come_from_service()
        {
            var contract = CreateProduct();
            this.mdmService.StubSearch<ProductScota>();

            const int TenorTypeId = 123;
            var productTenorType = new ProductTenorType { Details = { TenorType = TenorTypeId.ToEntityId() } };
            this.mdmService.StubSearch(productTenorType);

            var modelTenorType = this.mdmService.StubModel<TenorType, EnergyTrading.MDM.Client.Model.TenorType>(TenorTypeId);
            
            var product = this.service.Get(contract);

            Assert.AreEqual(1, product.TenorTypes.Count);
            Assert.AreEqual(modelTenorType, product.TenorTypes.First());
        }

        [TestMethod]
        public void Instrument_type_overrides_come_from_service()
        {
            var contract = CreateProduct();
            this.mdmService.StubSearch<ProductScota>();
            
            var productTenorType = new ProductTenorType();
            this.mdmService.StubSearch(productTenorType);

            var instrumentTypeOverride = new InstrumentTypeOverride();
            this.mdmService.StubSearch(instrumentTypeOverride);
            var modelInstrumentTypeOverride = this.mdmService.StubModel<InstrumentTypeOverride, EnergyTrading.MDM.Client.Model.InstrumentTypeOverride>(instrumentTypeOverride);

            var product = this.service.Get(contract);

            Assert.AreEqual(1, product.InstrumentTypeOverrides.Count);
            Assert.AreEqual(modelInstrumentTypeOverride, product.InstrumentTypeOverrides.First());
        }

        private static Product CreateProduct(Action<ProductDetails> configure = null)
        {
            var contract = new Product();

            if (configure != null)
            {
                configure(contract.Details);
            }

            return contract;
        }
    }
}