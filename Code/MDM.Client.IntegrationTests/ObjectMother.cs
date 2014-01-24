namespace MDM.Client.IntegrationTests
{
    using System;

    using RWEST.Nexus.MDM.Contracts;

    public class ObjectMother
    {
        public static T Create<T>()
            where T : IMdmEntity
        {
            var value = Create(typeof(T).Name);

            return (T)value;
        }

        public static IMdmEntity Create(string name)
        {
            switch (name)
            {
                case "Broker":
                    return CreateBroker();
                case "BrokerCommodity":
                    return CreateBrokerCommodity();
                case "BusinessUnit":
                    return CreateBusinessUnit();
                case "Calendar":
                    return CreateCalendar();
                case "Commodity":
                    return CreateCommodity();
                case "CommodityFeeType":
                    return CreateCommodityFeeType();
                case "CommodityInstrumentType":
                    return CreateCommodityInstrumentType();
                case "Counterparty":
                    return CreateCounterparty();
                case "Curve":
                    return CreateCurve();
                case "Dimension":
                    return CreateDimension();
                case "Exchange":
                    return CreateExchange();
                case "FeeType":
                    return CreateFeeType();
                case "Hierarchy":
                    return CreateHierarchy();
                case "InstrumentType":
                    return CreateInstrumentType();
                case "InstrumentTypeOverride":
                    return CreateInstrumentTypeOverride();
                case "Location":
                    return CreateLocation();
                case "Market":
                    return CreateMarket();
                case "Party":
                    return CreateParty();
                case "PartyAccountability":
                    return CreatePartyAccountability();
                case "PartyCommodity":
                    return CreatePartyCommodity();
                case "PartyOverride":
                    return CreatePartyOverride();
                case "PartyRole":
                    return CreatePartyRole();
                case "Person":
                    return CreatePerson();
                case "Portfolio":
                    return CreatePortfolio();
                case "PortfolioHierarchy":
                    return CreatePortfolioHierarchy();
                case "Product":
                    return CreateProduct();
                case "ProductCurve":
                    return CreateProductCurve();
                case "ProductDelivery":
                    return CreateProductDelivery();
                case "ProductScota":
                    return CreateProductScota();
                case "ProductSettlementCurve":
                    return CreateProductSettlementCurve();
                case "ProductType":
                    return CreateProductType();
                case "ProductTenorType":
                    return CreateProductTenorType();
                case "ProductTypeInstance":
                    return CreateProductTypeInstance();
                case "SettlementContact":
                    return CreateSettlementContact();
                case "Shape":
                    return CreateShape();
                case "ShapeDay":
                    return CreateShapeDay();
                case "ShapeElement":
                    return CreateShapeElement();
                case "ShipperCode":
                    return CreateShipperCode();
                case "SourceSystem":
                    return CreateSourceSystem();
                case "Tenor":
                    return CreateTenor();
                case "TenorType":
                    return CreateTenorType();
                case "Unit":
                    return CreateUnit();
                case "Vessel":
                    return CreateVessel();
                default:
                    throw new NotImplementedException("Unsupported MDM Entity type: " + name);
            }
        }

        private static IMdmEntity CreateCommodity()
        {
            var guid = Guid.NewGuid();

            var comm = new RWEST.Nexus.MDM.Contracts.Commodity
            {
                Identifiers = CreateIdList(guid),
                Details = new CommodityDetails
                              {
//                                  Name = "Coal"
                                  Name = "Commodity" + Guid.NewGuid()
                              },
            };

            return comm;
        }

        private static IMdmEntity CreateBroker()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.Broker
            {
                Identifiers = CreateIdList(guid), 
                Details = new BrokerDetails
                              {
                                  Name = "Broker" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateBrokerCommodity()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.BrokerCommodity
            {
                Identifiers = CreateIdList(guid),
                Details = new BrokerCommodityDetails
                              {
                                  Name = "BrokerCommodity" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateBusinessUnit()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.BusinessUnit
            {
                Identifiers = CreateIdList(guid),
                Details = new BusinessUnitDetails
                              {
                                  Name = "BusinessUnit" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateCalendar()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.Calendar
            {
                Identifiers = CreateIdList(guid),
                Details = new CalendarDetails
                              {
                                  Name = "Calendar" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateCommodityFeeType()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.CommodityFeeType
            {
                Identifiers = CreateIdList(guid),
                Details = new CommodityFeeTypeDetails()
            };
        }

        private static IMdmEntity CreateCommodityInstrumentType()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.CommodityInstrumentType
            {
                Identifiers = CreateIdList(guid),
                Details = new CommodityInstrumentTypeDetails()
            };
        }

        private static IMdmEntity CreateCounterparty()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.Counterparty
            {
                Identifiers = CreateIdList(guid),
                Details = new CounterpartyDetails
                              {
                                  Name = "Counterparty" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateCurve()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.Curve
            {
                Identifiers = CreateIdList(guid),
                Details = new CurveDetails
                              {
                                  Name = "Curve" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateDimension()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.Dimension
            {
                Identifiers = CreateIdList(guid),
                Details = new DimensionDetails
                              {
                                  Name = "Dimension" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateExchange()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.Exchange
            {
                Identifiers = CreateIdList(guid),
                Details = new ExchangeDetails
                              {
                                  Name = "Exchange" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateFeeType()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.FeeType
            {
                Identifiers = CreateIdList(guid),
                Details = new FeeTypeDetails
                              {
                                  Name = "FeeType" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateHierarchy()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.Hierarchy
            {
                Identifiers = CreateIdList(guid),
                Details = new HierarchyDetails
                              {
                                  Name = "Hierarchy" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateInstrumentType()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.InstrumentType
            {
                Identifiers = CreateIdList(guid),
                Details = new InstrumentTypeDetails
                              {
                                  Name = "InstrumentType" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateInstrumentTypeOverride()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.InstrumentTypeOverride
            {
                Identifiers = CreateIdList(guid),
                Details = new InstrumentTypeOverrideDetails
                              {
                                  Name = "InstrumentTypeOverride" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateMarket()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.Market
            {
                Identifiers = CreateIdList(guid),
                Details = new MarketDetails
                              {
                                  Name = "Market" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateParty()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.Party
            {
                Identifiers = CreateIdList(guid),
                Details = new PartyDetails
                              {
                                  Name = "Party" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreatePartyAccountability()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.PartyAccountability
            {
                Identifiers = CreateIdList(guid),
                Details = new PartyAccountabilityDetails
                              {
                                  Name = "PartyAccountability" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreatePartyCommodity()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.PartyCommodity
            {
                Identifiers = CreateIdList(guid),
                Details = new PartyCommodityDetails()
            };
        }

        private static IMdmEntity CreatePartyOverride()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.PartyOverride
            {
                Identifiers = CreateIdList(guid),
                Details = new PartyOverrideDetails()
            };
        }

        private static IMdmEntity CreatePartyRole()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.PartyRole
            {
                Identifiers = CreateIdList(guid),
                Details = new PartyRoleDetails
                              {
                                  Name = "PartyRole" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreatePerson()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.Person
            {
                Identifiers = CreateIdList(guid),
                Details = new PersonDetails
                              {
                                  Forename = "Person" + Guid.NewGuid(),
                                  Surname = "Person" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreatePortfolio()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.Portfolio
            {
                Identifiers = CreateIdList(guid),
                Details = new PortfolioDetails
                              {
                                  Name = "Portfolio" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreatePortfolioHierarchy()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.PortfolioHierarchy
            {
                Identifiers = CreateIdList(guid),
                Details = new PortfolioHierarchyDetails()
            };
        }

        private static IMdmEntity CreateProduct()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.Product
            {
                Identifiers = CreateIdList(guid),
                Details = new ProductDetails
                              {
                                  Name = "Product" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateProductCurve()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.ProductCurve
            {
                Identifiers = CreateIdList(guid),
                Details = new ProductCurveDetails
                              {
                                  Name = "ProductCurve" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateProductDelivery()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.ProductDelivery
            {
                Identifiers = CreateIdList(guid),
                Details = new ProductDeliveryDetails
                              {
                                  Name = "ProductDelivery" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateProductScota()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.ProductScota
            {
                Identifiers = CreateIdList(guid),
                Details = new ProductScotaDetails
                              {
                                  Name = "ProductScota" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateProductSettlementCurve()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.ProductSettlementCurve
            {
                Identifiers = CreateIdList(guid),
                Details = new ProductSettlementCurveDetails()
            };
        }

        private static IMdmEntity CreateProductType()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.ProductType
            {
                Identifiers = CreateIdList(guid),
                Details = new ProductTypeDetails
                              {
                                  Name = "ProductType" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateProductTenorType()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.ProductTenorType
            {
                Identifiers = CreateIdList(guid),
            };
        }

        private static IMdmEntity CreateProductTypeInstance()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.ProductTypeInstance
            {
                Identifiers = CreateIdList(guid),
                Details = new ProductTypeInstanceDetails
                              {
                                  Name = "ProductTypeInstance" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateSettlementContact()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.SettlementContact
            {
                Identifiers = CreateIdList(guid),
                Details = new SettlementContactDetails
                              {
                                  Name = "SettlementContact" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateShape()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.Shape
            {
                Identifiers = CreateIdList(guid),
                Details = new ShapeDetails
                              {
                                  Name = "Shape" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateShapeDay()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.ShapeDay
            {
                Identifiers = CreateIdList(guid),
                Details = new ShapeDayDetails()
            };
        }

        private static IMdmEntity CreateShapeElement()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.ShapeElement
            {
                Identifiers = CreateIdList(guid),
                Details = new ShapeElementDetails
                              {
                                  Name = "ShapeElement" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateShipperCode()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.ShipperCode
            {
                Identifiers = CreateIdList(guid),
                Details = new ShipperCodeDetails()
            };
        }

        private static IMdmEntity CreateSourceSystem()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.SourceSystem
            {
                Identifiers = CreateIdList(guid),
                Details = new SourceSystemDetails
                              {
                                  Name = "SourceSystem" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateTenor()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.Tenor
            {
                Identifiers = CreateIdList(guid),
                Details = new TenorDetails
                {
                    Name = "Tenor" + Guid.NewGuid()
                },
            };
        }

        private static IMdmEntity CreateTenorType()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.TenorType
            {
                Identifiers = CreateIdList(guid),
                Details = new TenorTypeDetails
                {
                    Name = "TenorType" + Guid.NewGuid()
                },
            };
        }

        private static IMdmEntity CreateUnit()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.Unit
            {
                Identifiers = CreateIdList(guid),
                Details = new UnitDetails
                              {
                                  Name = "Unit" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateVessel()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.Vessel
            {
                Identifiers = CreateIdList(guid),
                Details = new VesselDetails
                              {
                                  Name = "Vessel" + Guid.NewGuid()
                              },
            };
        }

        private static IMdmEntity CreateLocation()
        {
            var guid = Guid.NewGuid();

            return new RWEST.Nexus.MDM.Contracts.Location
            {
                Identifiers = CreateIdList(guid),
                Details = new LocationDetails
                              {
                                  Name = "Location" + Guid.NewGuid()
                              },
            };
        }

        private static NexusIdList CreateIdList(Guid guid)
        {
            return new NexusIdList
                       {
                           new NexusId { SystemName = "Endur", Identifier = "Endur" + guid },
                           new NexusId { SystemName = "Trayport", Identifier = "Trayport" + guid },
                       };
        }
    }
}