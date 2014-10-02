using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using EnergyTrading.Caching;
using EnergyTrading.Caching.InMemory;
using EnergyTrading.Mdm.Client.Extensions;
using EnergyTrading.Mdm.Client.Services;
using EnergyTrading.Mdm.Contracts;
using Microsoft.Practices.ObjectBuilder2;
using Moq;
using NUnit.Framework;

namespace EnergyTrading.Mdm.Client.Tests.Services
{
    [TestFixture]
    public class MdmCacheServiceFixture
    {
        private DefaultMdmClientCacheService mdmClientCacheService;
        private InMemoryCacheTestClass inMemoryTestClass;

        [SetUp]
        public void SetupTest()
        {
            inMemoryTestClass = new InMemoryCacheTestClass();
            mdmClientCacheService = new DefaultMdmClientCacheService(inMemoryTestClass);
        }

        [Test]
        public void WhenEntityIsAddedIntoCacheTheNumberOfCacheItemsShouldEqualToNumberOfMappingsPlus2()
        {
            var testMdmObject = GetTestObject<SourceSystem>();
            mdmClientCacheService.Add(testMdmObject);
            Assert.AreEqual( testMdmObject.Identifiers.Count(a => !a.IsMdmId) + 2,inMemoryTestClass.CachedItemsCount);
        }


        [Test]
        public void WhenEntityReferedByMappingExpiresThenMappingShouldBeRemovedFromCache()
        {
            var testMdmObject = GetTestObject<SourceSystem>();
            mdmClientCacheService.Add(testMdmObject);

            //Expires
            inMemoryTestClass.Remove(testMdmObject.ToMdmKey().ToString());

            var cachedTotalMappings = testMdmObject.Identifiers.Count(a=>!a.IsMdmId);
            const int cachedTotalEntityIdToMapList = 1;

            Assert.AreEqual(cachedTotalMappings + cachedTotalEntityIdToMapList, inMemoryTestClass.CachedItemsCount);

            var entity = mdmClientCacheService.Get<SourceSystem>(testMdmObject.Identifiers[0]);

            Assert.IsNull(entity);

            Assert.AreEqual((cachedTotalMappings - 1) + cachedTotalEntityIdToMapList, inMemoryTestClass.CachedItemsCount);
        }

        [Test]
        public void WhenEntityIsClearedNoItemsShouldExistsInCache()
        {
            var testMdmObject = GetTestObject<SourceSystem>();
            mdmClientCacheService.Add(testMdmObject);

            Assert.AreEqual(4, inMemoryTestClass.CachedItemsCount);
            mdmClientCacheService.Remove(testMdmObject.ToMdmKey());

            Assert.AreEqual(0, inMemoryTestClass.CachedItemsCount);
        }


        [Test]
        public void WhenMappingIsRemovedFromCachedEntityAndCallingOfGetWithRemovedMdmIdShouldReturnNullAndMappingShouldGetDeleted()
        {
            var testMdmObject = GetTestObject<SourceSystem>();
            mdmClientCacheService.Add(testMdmObject);

            var mappingToBeRemoved = testMdmObject.Identifiers[0];
            testMdmObject.Identifiers.Remove(mappingToBeRemoved);
            mdmClientCacheService.Add(testMdmObject);

            //4=2 Mappings + 1 Entity + 1 EnityToMapList
            Assert.AreEqual(4, inMemoryTestClass.CachedItemsCount); 

            Assert.IsNull(mdmClientCacheService.Get<SourceSystem>(mappingToBeRemoved));

            //1 Mapping + 1 Entity + 1 EnityToMapList
            Assert.AreEqual(3, inMemoryTestClass.CachedItemsCount);
        }


        [Test]
        public void WhenSameObjectIsStoredTwiceThereShouldBeAnyDuplicatesInCache()
        {
            var testMdmObject1 = GetTestObject<SourceSystem>();

            mdmClientCacheService.Add(testMdmObject1);
            mdmClientCacheService.Add(testMdmObject1);

            //4=2 Mappings + 1 Entity + 1 EnityToMapList
            Assert.AreEqual(4, inMemoryTestClass.CachedItemsCount);

            testMdmObject1.Identifiers.Add(new MdmId { SystemName = "XYZ", Identifier = "aaa" });
            mdmClientCacheService.Add(testMdmObject1);

            //5=3 Mappings + 1 Entity + 1 EnityToMapList
            Assert.AreEqual(5, inMemoryTestClass.CachedItemsCount);
        }

        [Test]
        public void WhenTwoObjectsAreStoredThereShouldnotBeAnyConflicts()
        {
            var testMdmObject1 = GetTestObject<SourceSystem>();
            var testMdmObject2 = GetTestObject<SourceSystem>();

            mdmClientCacheService.Add(testMdmObject1);
            
            //4=2 Mappings + 1 Entity + 1 EnityToMapList
            Assert.AreEqual(4, inMemoryTestClass.CachedItemsCount); 
            mdmClientCacheService.Add(testMdmObject2);
            Assert.AreEqual(8, inMemoryTestClass.CachedItemsCount);

            mdmClientCacheService.Remove(testMdmObject1.ToMdmKey());
            mdmClientCacheService.Remove(testMdmObject2.ToMdmKey());
            Assert.AreEqual(0, inMemoryTestClass.CachedItemsCount);
        }

        



        private TContract GetTestObject<TContract>() where TContract : class,IMdmEntity, new()
        {
            var sourceSystem = new TContract
            {
                Identifiers =
                    new MdmIdList
                                       {
                                           new MdmId
                                           {
                                               SystemName = "ABC",
                                               Identifier = Guid.NewGuid().ToString()
                                           },
                                           new MdmId
                                           {
                                               SystemName = "XYZ",
                                               Identifier = Guid.NewGuid().ToString()
                                           },
                                           new MdmId
                                           {
                                               SystemName = "Nexus", Identifier = Guid.NewGuid().GetHashCode().ToString(), IsMdmId = true
                                           }
                                       },
                //Details = { Name = "TestSystem" }
            };

            return sourceSystem;
        }

        class InMemoryCacheTestClass : ICacheService
        {
            private readonly MemoryCache cache;

            public int CachedItemsCount
            {
                get { return cache.Count(); }
            }

            public void RemoveByValue<T>(T value)
            {
                (from key in cache where Equals(key.Value, value) select key.Key).ForEach(
                    key => cache.Remove(key));
            }

            public InMemoryCacheTestClass() : this(new MemoryCache("Test")) { }
            
            public InMemoryCacheTestClass(MemoryCache cache)
            {
                this.cache = cache;
            }

            public bool Remove(string key)
            {
                return cache.Remove(key) != null;
            }

            public void Add<T>(string key, T value, CacheItemPolicy policy = null)
            {
                cache.Add(key, value, policy);
            }

            public T Get<T>(string key)
            {
                return (T)cache.Get(key);
            }
        }


    }
}
