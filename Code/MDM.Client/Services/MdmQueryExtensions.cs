namespace EnergyTrading.MDM.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using EnergyTrading.MDM.Client.Extensions;
    using EnergyTrading.MDM.Client.Model;

    using EnergyTrading.Contracts.Search;
    using RWEST.Nexus.MDM.Contracts;
    using EnergyTrading.Search;

    using Calendar = RWEST.Nexus.MDM.Contracts.Calendar;
    using InstrumentTypeOverride = RWEST.Nexus.MDM.Contracts.InstrumentTypeOverride;
    using ProductScota = RWEST.Nexus.MDM.Contracts.ProductScota;
    using SearchExtensions = RWEST.Nexus.Contracts.Search.SearchExtensions;
    using TenorType = RWEST.Nexus.MDM.Contracts.TenorType;

    /// <summary>
    /// Standard MDM queries and helper methods for retrieving MDM and model entities.
    /// </summary>
    public static class MdmQueryExtensions
    {
        public static RWEST.Nexus.MDM.Contracts.Agreement Agreement(this IMdmModelEntityService service, int agreementId)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.Agreement>(agreementId);
        }

        public static RWEST.Nexus.MDM.Contracts.Book Book(this IMdmModelEntityService service, int bookId)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.Book>(bookId);
        }

        public static RWEST.Nexus.MDM.Contracts.Broker Broker(this IMdmModelEntityService service, int brokerId)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.Broker>(brokerId);
        }

        public static Calendar Calendar(this IMdmModelEntityService service, int calendarId)
        {
            return service.Get<Calendar>(calendarId);
        }

        public static RWEST.Nexus.MDM.Contracts.Curve Curve(this IMdmModelEntityService service, int curveId)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.Curve>(curveId);
        }

        public static BookDefault BookDefault(this IMdmModelEntityService service, int bookDefaultId)
        {
            return service.Get<BookDefault>(bookDefaultId);
        }

        public static RWEST.Nexus.MDM.Contracts.Commodity Commodity(this IMdmModelEntityService service, int commodityId)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.Commodity>(commodityId);
        }

        public static RWEST.Nexus.MDM.Contracts.CommodityInstrumentType CommodityInstrumentType(this IMdmModelEntityService service, int commodityInstrumentTypeId)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.CommodityInstrumentType>(commodityInstrumentTypeId);
        }

        public static RWEST.Nexus.MDM.Contracts.CommodityInstrumentType CommodityInstrumentType<T>(this IMdmModelEntityService service, T entity, Func<T, EntityId> access)
            where T : class, IMdmEntity
        {
            return service.CommodityInstrumentType(entity.ToMdmKey(access));
        }

        public static RWEST.Nexus.MDM.Contracts.Exchange Exchange(this IMdmModelEntityService service, int exchangeId)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.Exchange>(exchangeId);
        }

        public static InstrumentType InstrumentType(this IMdmModelEntityService service, int instrumentTypeId)
        {
            return service.Get<InstrumentType>(instrumentTypeId);
        }

        public static Location Location(this IMdmModelEntityService service, int locationId)
        {
            return service.Get<Location>(locationId);
        }

        public static RWEST.Nexus.MDM.Contracts.Market Market(this IMdmModelEntityService service, int marketId)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.Market>(marketId);
        }

        public static Party Party(this IMdmModelEntityService service, int partyId)
        {
            return service.Get<Party>(partyId);
        }

        public static IList<PartyAccountability> PartyAccountability(this IMdmModelEntityService service, int partyId, string partyAccountabilityType)
        {
            var search = SearchBuilder.CreateSearch();
            search.AddSearchCriteria(SearchCombinator.And)
                .AddCriteria("TargetParty.Id", SearchCondition.Equals, partyId.ToString(CultureInfo.InvariantCulture), true)
                .AddCriteria("PartyAccountabilityType", SearchCondition.Equals, partyAccountabilityType, false);

            var results = service.Search<PartyAccountability>(search);

            return !results.IsValid ? null : results.Message;
        }

        public static PartyRole PartyRole(this IMdmModelEntityService service, int partyId)
        {
            return service.Get<PartyRole>(partyId);
        }

        public static PartyRole PartyRole<T>(this IMdmModelEntityService service, T entity, Func<T, EntityId> access)
            where T : class, IMdmEntity
        {
            return service.Get<T, PartyRole>(entity, access);
        }

        public static IList<PartyRoleAccountability> PartyRoleAccountability(this IMdmModelEntityService service, int partyId, string partyRoleAccountabilityType)
        {
            var search = SearchBuilder.CreateSearch();
            search.AddSearchCriteria(SearchCombinator.And)
                .AddCriteria("TargetParty.Id", SearchCondition.Equals, partyId.ToString(CultureInfo.InvariantCulture), true)
                .AddCriteria("PartyRoleAccountabilityType", SearchCondition.Equals, partyRoleAccountabilityType, false);

            var results = service.Search<PartyRoleAccountability>(search);

            return !results.IsValid ? null : results.Message;
        }

        public static RWEST.Nexus.MDM.Contracts.Product Product(this IMdmModelEntityService service, int id)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.Product>(id);
        }

        public static RWEST.Nexus.MDM.Contracts.Product Product(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.ProductTypeInstance productTypeInstance)
        {
            var productTypeId = productTypeInstance.ToMdmKey(x => x.Details.ProductType);
            var productType = ProductType(service, productTypeId);

            return Product(service, productType.ToMdmKey(x => x.Details.Product));
        }

        public static RWEST.Nexus.MDM.Contracts.ProductCurve ProductCurve(this IMdmModelEntityService service, int id)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.ProductCurve>(id);
        }

        public static RWEST.Nexus.MDM.Contracts.ProductScota ProductScota(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.Product product)
        {
            var search = SearchBuilder.CreateSearch();
            search.AddSearchCriteria(SearchCombinator.And)
                  .AddMdmIdCriteria("Product.Id", product);

            var results = service.Search<RWEST.Nexus.MDM.Contracts.ProductScota>(search);
            if (results.IsValid && results.Message != null && results.Message.Any())
            {
                return results.Message.First();
            }

            return null;
        }

        public static IList<ProductTenorType> ProductTenorTypes(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.Product product)
        {
            var search = SearchBuilder.CreateSearch();
            search.AddSearchCriteria(SearchCombinator.And)
                  .AddMdmIdCriteria("Product.Id", product);

            var results = service.Search<RWEST.Nexus.MDM.Contracts.ProductTenorType>(search);
            return results.HandleResponse();
        }

        public static ProductTenorType ProductTenorType(this IMdmModelEntityService service, int id)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.ProductTenorType>(id);
        }

        public static IList<TenorType> TenorTypes(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.Product product)
        {
            var productTenorTypes = service.ProductTenorTypes(product);
            return productTenorTypes.Select(ptt => service.Get<TenorType>(ptt.Details.TenorType.ToNexusKey())).ToList();
        }

        public static RWEST.Nexus.MDM.Contracts.ProductType ProductType(this IMdmModelEntityService service, int id)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.ProductType>(id);
        }

        public static RWEST.Nexus.MDM.Contracts.ProductTypeInstance ProductTypeInstance(this IMdmModelEntityService service, int id)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.ProductTypeInstance>(id);
        }

        public static Shape Shape(this IMdmModelEntityService service, int id)
        {
            return service.Get<Shape>(id);
        }

        public static IList<Tenor> TenorTypeTenors(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.TenorType tenorType)
        {
            var search = SearchBuilder.CreateSearch();
            search.AddSearchCriteria(SearchCombinator.And)
                  .AddMdmIdCriteria("TenorType.Id", tenorType);

            return service.Search<RWEST.Nexus.MDM.Contracts.Tenor>(search).HandleResponse();
        }

        public static Client.Model.Broker ModelBroker<T>(this IMdmModelEntityService service, T entity, Func<T, EntityId> access)
            where T : class, IMdmEntity
        {
            return service.Model<T, RWEST.Nexus.MDM.Contracts.Broker, Client.Model.Broker>(entity, access);
        }

        public static Client.Model.BrokerRate ModelBrokerRate(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.BrokerRate brokerRate)
        {
            if (brokerRate == null)
            {
                return null;
            }

            var model = new Client.Model.BrokerRate
            {
                Source = brokerRate,
                Broker = Broker(service, brokerRate.ToMdmKey(x => x.Details.Broker)),
                Desk = PartyRole(service, brokerRate.ToMdmKey(x => x.Details.Desk)),
                CommodityInstrumentType = CommodityInstrumentType(service, brokerRate.ToMdmKey(x => x.Details.CommodityInstrumentType)),
                Location = Location(service, brokerRate.ToMdmKey(x => x.Details.Location)),
                ProductType = ProductType(service, brokerRate.ToMdmKey(x => x.Details.ProductType)),
                PartyAction = brokerRate.Details.PartyAction,
                Rate = brokerRate.Details.Rate,
                RateType = brokerRate.Details.RateType,
                Currency = brokerRate.Details.Currency
            };

            return model;
        }

        public static Client.Model.Commodity ModelCommodity<T>(this IMdmModelEntityService service, T entity, Func<T, EntityId> access)
            where T : class, IMdmEntity
        {
            return service.Model<T, RWEST.Nexus.MDM.Contracts.Commodity, Client.Model.Commodity>(entity, access);
        }

        public static Client.Model.CommodityInstrumentType ModelCommodityInstrumentType(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.CommodityInstrumentType entity)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.CommodityInstrumentType, Client.Model.CommodityInstrumentType>(entity);
        }

        public static Client.Model.CommodityInstrumentType ModelCommodityInstrumentType(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.CommodityInstrumentType contract, string instrumentSubType)
        {
            if (contract == null)
            {
                return null;
            }

            var model = service.ModelCommodityInstrumentType(contract);
            if (model != null)
            {
                model.InstrumentSubType = instrumentSubType;
            }

            return model;
        }

        public static Client.Model.CommodityInstrumentType ModelCommodityInstrumentType<T>(this IMdmModelEntityService service, T entity, Func<T, EntityId> access, string instrumentSubType)
            where T : class, IMdmEntity
        {
            return service.Model<T, RWEST.Nexus.MDM.Contracts.CommodityInstrumentType, Client.Model.CommodityInstrumentType>(entity, access);
        }

        public static Client.Model.Curve ModelCurve<T>(this IMdmModelEntityService service, T entity, Func<T, EntityId> access)
            where T : class, IMdmEntity
        {
            return service.Model<T, RWEST.Nexus.MDM.Contracts.Curve, Client.Model.Curve>(entity, access);
        }

        public static Client.Model.Curve ModelCurve(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.Curve curve)
        {
            if (curve == null)
            {
                return null;
            }

            var model = new Client.Model.Curve
            {
                Source = curve,
                Commodity = ModelCommodity(service, curve, x => x.Details.Commodity),
                Location = Location(service, curve.ToMdmKey(x => x.Details.Location)),
                Originator = Party(service, curve.ToMdmKey(x => x.Details.Originator))
            };

            return model;
        }

        public static Client.Model.Exchange ModelExchange(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.Exchange exchange)
        {
            if (exchange == null)
            {
                return null;
            }

            var model = new Client.Model.Exchange
            {
                Source = exchange,
                Name = exchange.Details.Name
            };

            return model;
        }

        public static Client.Model.Exchange ModelExchange<T>(this IMdmModelEntityService service, T entity, Func<T, EntityId> access)
            where T : class, IMdmEntity
        {
            return service.Model<T, RWEST.Nexus.MDM.Contracts.Exchange, Client.Model.Exchange>(entity, access);
        }

        public static Client.Model.InstrumentTypeOverride ModelInstrumentTypeOverride(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.InstrumentTypeOverride entity)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.InstrumentTypeOverride, Client.Model.InstrumentTypeOverride>(entity);
        }

        public static List<Client.Model.InstrumentTypeOverride> ModelInstrumentTypeOverrides(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.Product product)
        {
            var productTenorTypes = service.ProductTenorTypes(product);

            if (!productTenorTypes.Any())
            {
                return new List<Client.Model.InstrumentTypeOverride>();
            }

            var itoSearch = SearchBuilder.CreateSearch();
            itoSearch.AddSearchCriteria(SearchCombinator.Or);
            var criteria = itoSearch.SearchFields.Criterias[0];

            foreach (var ptt in productTenorTypes)
            {
                criteria.AddMdmIdCriteria("ProductTenorType.Id", ptt);
            }

            return service.Search<RWEST.Nexus.MDM.Contracts.InstrumentTypeOverride>(itoSearch)
                          .HandleResponse()
                          .Select(x => service.ModelInstrumentTypeOverride(x)).ToList();
        }
        
        public static List<Client.Model.InstrumentTypeOverride> ModelInstrumentTypeOverrides(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.ProductType productType)
        {
            var search = SearchBuilder.CreateSearch();
            search.AddSearchCriteria(SearchCombinator.And)
                  .AddMdmIdCriteria("ProductType.Id", productType);

            var results = service.Search<RWEST.Nexus.MDM.Contracts.InstrumentTypeOverride>(search);

            var modelInstrumentTypeOverrides = new List<Client.Model.InstrumentTypeOverride>();

            if (results.IsValid)
            {
                modelInstrumentTypeOverrides.AddRange(results.Message.Select(x => service.ModelInstrumentTypeOverride(x)));
            }

            return modelInstrumentTypeOverrides;
        }

        public static Client.Model.Market ModelMarket<T>(this IMdmModelEntityService service, T entity, Func<T, EntityId> access)
            where T : class, IMdmEntity
        {
            return service.Model<T, RWEST.Nexus.MDM.Contracts.Market, Client.Model.Market>(entity, access);
        }

        public static Client.Model.Product ModelProduct(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.Product entity)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.Product, Client.Model.Product>(entity);
        }

        public static Client.Model.ProductDelivery ModelProductDelivery(this IMdmModelEntityService service, NexusId id, string productHierarchyLevel = null)
        {
            var pti = (string.IsNullOrWhiteSpace(productHierarchyLevel)
                       || productHierarchyLevel == ProductHierarchyLevel.ProductTypeInstance)
                          ? service.Get<RWEST.Nexus.MDM.Contracts.ProductTypeInstance>(id)
                          : null;

            var pt = ((string.IsNullOrWhiteSpace(productHierarchyLevel) && pti == null)
                      || productHierarchyLevel == ProductHierarchyLevel.ProductType)
                         ? service.Get<RWEST.Nexus.MDM.Contracts.ProductType>(id)
                         : null;

            var p = ((string.IsNullOrEmpty(productHierarchyLevel) && pt == null)
                       || productHierarchyLevel == ProductHierarchyLevel.Product)
                         ? service.Get<RWEST.Nexus.MDM.Contracts.Product>(id)
                         : null;

            if (pti == null && pt == null && p == null)
            {
                return null;
            }

            var model = new Client.Model.ProductDelivery
            {
                Product = service.ModelProduct(p),
                ProductType = service.ModelProductType(pt),
                ProductTypeInstance = service.ModelProductTypeInstance(pti)
            };

            return model;
        }

        public static Client.Model.Product ModelProduct<T>(this IMdmModelEntityService service, T entity, Func<T, EntityId> access)
            where T : class, IMdmEntity
        {
            return service.Model<T, RWEST.Nexus.MDM.Contracts.Product, Client.Model.Product>(entity, access);
        }

        public static Client.Model.Product ModelProductForStandingData(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.Product product, IList<InstrumentTypeOverride> overrides, IList<ProductScota> productScotas)
        {
            if (product == null)
            {
                return null;
            }

            var model = new Client.Model.Product
            {
                Source = product,
                Market = ModelMarket(service, product, x => x.Details.Market),
                CommodityInstrumentType = ModelCommodityInstrumentType(service, CommodityInstrumentType(service, product.ToMdmKey(x => x.Details.CommodityInstrumentType)), product.Details.InstrumentSubType),
                ProductCurves = ModelProductCurves(service, product),
                InstrumentTypeOverrides = ModelInstrumentTypeOverrides(service, product, overrides),
                ScotaTerms = ModelScotaTerms(service, product, productScotas)
            };

            return model;
        }

        private static List<Client.Model.InstrumentTypeOverride> ModelInstrumentTypeOverrides(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.Product product, IList<InstrumentTypeOverride> overrides)
        {
            var modelInstrumentTypeOverrides = new List<Client.Model.InstrumentTypeOverride>();

            if (overrides != null)
            {
                modelInstrumentTypeOverrides.AddRange(overrides.Select(o => ModelInstrumentTypeOverride(service, o)));

                modelInstrumentTypeOverrides.RemoveAll(c => c.ProductId != product.Identifiers.PrimaryIdentifier().Identifier);
            }

            return modelInstrumentTypeOverrides;
        }

        public static Client.Model.ProductCurve ModelProductCurve(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.ProductCurve entity)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.ProductCurve, Client.Model.ProductCurve>(entity);
        }

        public static List<Client.Model.ProductCurve> ModelProductCurves(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.Product product)
        {
            var modelProductCurves = new List<Client.Model.ProductCurve>();

            foreach (var productCurveId in product.Links.ReferencedIds<RWEST.Nexus.MDM.Contracts.ProductCurve>())
            {
                var productCurve = service.ProductCurve(productCurveId);

                var model = service.ModelProductCurve(productCurve);
                modelProductCurves.Add(model);
            }

            return modelProductCurves;
        }

        public static Client.Model.ProductScota ModelProductScota(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.ProductScota entity)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.ProductScota, Client.Model.ProductScota>(entity);
        }

        public static Client.Model.ProductType ModelProductType(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.ProductType entity)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.ProductType,  Client.Model.ProductType>(entity);
        }

        public static Client.Model.ProductType ModelProductType<T>(this IMdmModelEntityService service, T entity, Func<T, EntityId> access)
            where T : class, IMdmEntity
        {
            return service.Model<T, RWEST.Nexus.MDM.Contracts.ProductType, Client.Model.ProductType>(entity, access);
        }

        public static Client.Model.ProductTypeInstance ModelProductTypeInstance(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.ProductTypeInstance entity)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.ProductTypeInstance, Client.Model.ProductTypeInstance>(entity);
        }

        private static ScotaTerms ModelScotaTerms(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.Product product, IEnumerable<ProductScota> productScotas)
        {
            if (productScotas == null)
            {
                return null;
            }

            var scotaTerms = productScotas
                .Where(x => x.Details.Product.Identifier.Identifier == product.ToMdmId().Identifier)
                .Select(ps => ModelScotaTerms(service, ps)).FirstOrDefault();

            return scotaTerms;
        }

        private static ScotaTerms ModelScotaTerms(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.ProductScota productScota)
        {
            if (productScota == null)
            {
                return null;
            }

            var model = new ScotaTerms
            {
                Source = productScota,
                Rss = productScota.Details.ScotaRss,
                Version = productScota.Details.ScotaVersion,
                Contract = productScota.Details.ScotaContract,
                DeliveryPoint = Location(service, productScota.ToMdmKey(x => x.Details.ScotaDeliveryPoint)),
                Origin = Location(service, productScota.ToMdmKey(x => x.Details.ScotaOrigin))
            };

            return model;
        }

        public static Client.Model.ShipperCode ModelShipperCode(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.ShipperCode shipperCode)
        {
            if (shipperCode == null)
            {
                return null;
            }

            var model = new Client.Model.ShipperCode
            {
                Location = Location(service, shipperCode.ToMdmKey(x => x.Details.Location)),
                Party = Party(service, shipperCode.ToMdmKey(x => x.Details.Party)),
                Source = shipperCode
            };

            return model;
        }

        public static Client.Model.TenorType ModelTenorType(this IMdmModelEntityService service, RWEST.Nexus.MDM.Contracts.TenorType entity)
        {
            return service.Get<RWEST.Nexus.MDM.Contracts.TenorType, Client.Model.TenorType>(entity);
        }

        public static List<Client.Model.TenorType> ModelTenorTypes(this IMdmModelEntityService service, IEnumerable<TenorType> entities)
        {
            return entities.Select(entity => service.ModelTenorType(entity)).ToList();
        }

        /// <summary>
        /// Get a MDM contract using data in another entity
        /// </summary>
        /// <typeparam name="T">Type of the entity to use.</typeparam>
        /// <typeparam name="TContract">Type of the contract to use</typeparam>
        /// <param name="service">MDM service to use</param>
        /// <param name="entity">Entity to use</param>
        /// <param name="access">Function to return the identity of the contract from the entity.</param>
        /// <returns>MDM contract if found, otherwise null.</returns>
        public static TContract Get<T, TContract>(this IMdmModelEntityService service, T entity, Func<T, EntityId> access)
            where T : class, IMdmEntity
            where TContract : class, IMdmEntity
        {
            return service.Get<TContract>(entity.ToMdmKey(access));
        }

        public static TModel Model<TContract, TModel>(this IMdmModelEntityService service, TContract entity)
            where TContract : class, IMdmEntity
            where TModel : IMdmModelEntity<TContract>
        {
            return service.Get<TContract, TModel>(entity);
        }

        public static TModel Model<T, TContract, TModel>(this IMdmModelEntityService service, T entity, Func<T, EntityId> access)
            where T : class, IMdmEntity
            where TContract : class, IMdmEntity
            where TModel : IMdmModelEntity<TContract>
        {
            return service.Get<TContract, TModel>(service.Get<T, TContract>(entity, access));
        }
    }
}