namespace EnergyTrading.MDM.Client.Tests.Services.Model
{
    using System.Globalization;

    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.Services.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class ModelInstrumentTypeOverrideServiceFixture
    {
        private Mock<IMdmModelEntityService> mdmService;
        private ModelInstrumentTypeOverrideService service;

        [TestInitialize]
        public void SetUp()
        {
            this.mdmService = new Mock<IMdmModelEntityService>();
            this.service = new ModelInstrumentTypeOverrideService(this.mdmService.Object);
        }

        [TestMethod]
        public void Return_null_if_contract_is_null()
        {
            var instrumentTypeOverride = this.service.Get(null);

            Assert.IsNull(instrumentTypeOverride);
        }

        [TestMethod]
        public void Source_is_contract()
        {
            var contract = new InstrumentTypeOverride
            {
                Details = new InstrumentTypeOverrideDetails
                {
                    ProductType = 1.ToEntityId(),
                }
            };
            this.StubProductType(1, "id");

            var instrumentTypeOverride = this.service.Get(contract);

            Assert.AreEqual(contract, instrumentTypeOverride.Source);
        }

        [TestMethod]
        public void Name_comes_from_contract_details()
        {
            const string Name = "name";
            var contract = new InstrumentTypeOverride
            {
                Details = new InstrumentTypeOverrideDetails
                {
                    Name = Name,
                    ProductType = 1.ToEntityId(),
                }
            };
            this.StubProductType(1, "id");

            var instrumentTypeOverride = this.service.Get(contract);

            Assert.AreEqual(Name, instrumentTypeOverride.Name);
        }

        [TestMethod]
        public void Product_id_comes_from_product_type_from_service()
        {
            const int ProductTypeId = 123;
            var contract = new InstrumentTypeOverride
            {
                Details = new InstrumentTypeOverrideDetails
                {
                    ProductType = ProductTypeId.ToEntityId(),
                }
            };
            const string ProductId = "productId";
            this.StubProductType(ProductTypeId, ProductId);

            var instrumentTypeOverride = this.service.Get(contract);

            Assert.AreEqual(ProductId, instrumentTypeOverride.ProductId);
        }

        [TestMethod]
        public void Product_type_id_comes_from_product_type_from_service()
        {
            const int ProductTypeId = 123;
            var contract = new InstrumentTypeOverride
            {
                Details = new InstrumentTypeOverrideDetails
                {
                    ProductType = ProductTypeId.ToEntityId(),
                }
            };
            this.StubProductType(ProductTypeId, "productId");

            var instrumentTypeOverride = this.service.Get(contract);

            Assert.AreEqual(ProductTypeId.ToString(CultureInfo.InvariantCulture), instrumentTypeOverride.ProductTypeId);
        }

        [TestMethod]
        public void Product_type_is_optional()
        {
            var contract = new InstrumentTypeOverride();
            var instrumentTypeOverride = this.service.Get(contract);
            Assert.IsNull(instrumentTypeOverride.ProductId);
            Assert.IsNull(instrumentTypeOverride.ProductTypeId);
        }

        [TestMethod]
        public void Broker_comes_from_service()
        {
            const int BrokerId = 123;
            var contract = new InstrumentTypeOverride
            {
                Details = new InstrumentTypeOverrideDetails
                {
                    Broker = BrokerId.ToEntityId(),
                    ProductType = 1.ToEntityId(),
                }
            };
            this.StubProductType(1, "productId");
            var broker = new Broker();
            this.mdmService.Setup(s => s.Get<Broker>(BrokerId)).Returns(broker);
            var modelBroker = new EnergyTrading.MDM.Client.Model.Broker();
            this.mdmService.Setup(s => s.Get<Broker, EnergyTrading.MDM.Client.Model.Broker>(broker)).Returns(modelBroker);

            var instrumentTypeOverride = this.service.Get(contract);

            Assert.AreEqual(modelBroker, instrumentTypeOverride.Broker);
        }

        [TestMethod]
        public void Commodity_instrument_type_comes_from_service()
        {
            const int CommodityInstrumentTypeId = 123;
            var contract = new InstrumentTypeOverride
            {
                Details = new InstrumentTypeOverrideDetails
                {
                    CommodityInstrumentType = CommodityInstrumentTypeId.ToEntityId(),
                    ProductType = 1.ToEntityId(),
                }
            };
            this.StubProductType(1, "productId");
            var commodityInstrumentType = new CommodityInstrumentType();
            this.mdmService.Setup(s => s.Get<CommodityInstrumentType>(CommodityInstrumentTypeId)).Returns(commodityInstrumentType);
            var modelCommodityInstrumentType = new EnergyTrading.MDM.Client.Model.CommodityInstrumentType();
            this.mdmService.Setup(s => s.Get<CommodityInstrumentType, EnergyTrading.MDM.Client.Model.CommodityInstrumentType>(commodityInstrumentType))
                .Returns(modelCommodityInstrumentType);

            var instrumentTypeOverride = this.service.Get(contract);

            Assert.AreEqual(modelCommodityInstrumentType, instrumentTypeOverride.CommodityInstrumentType);
        }

        [TestMethod]
        public void Tenor_type_comes_from_service()
        {
            const int ProductTenorTypeId = 123;
            var contract = new InstrumentTypeOverride
                               {
                                   Details =
                                       new InstrumentTypeOverrideDetails
                                           {
                                               ProductTenorType = ProductTenorTypeId.ToEntityId()
                                           }
                               };

            const int TenorTypeId = 456;
            this.mdmService.Setup(x => x.Get<ProductTenorType>(ProductTenorTypeId)).Returns(new ProductTenorType
                                                                                           {
                                                                                               Details =
                                                                                                   {
                                                                                                       TenorType = TenorTypeId.ToEntityId()
                                                                                                   }
                                                                                           });

            var instrumentTypeOverride = this.service.Get(contract);

            Assert.AreEqual(TenorTypeId.ToString(), instrumentTypeOverride.TenorTypeId);
        }

        private void StubProductType(int productTypeId, string productId)
        {
            var productType = new ProductType
            {
                Details = new ProductTypeDetails
                {
                    Product = new EntityId
                    {
                        Identifier = new NexusId
                        {
                            Identifier = productId
                        }
                    }
                },
                Identifiers = new NexusIdList
                {
                    new NexusId
                    {
                        Identifier = productTypeId.ToString(CultureInfo.InvariantCulture)
                    }
                }
            };
            this.mdmService.Setup(s => s.Get<ProductType>(productTypeId)).Returns(productType);
        }
    }
}