namespace EnergyTrading.MDM.Client.Tests.Services
{
    using System;
    using System.Collections.Specialized;
    using System.Net;

    using EnergyTrading.Caching;
    using EnergyTrading.Configuration;
    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.WebClient;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using RWEST.Nexus.MDM.Contracts;

    using SharpTestsEx;

    [TestClass]
    public class CachePolicyMdmEntityServiceFixture
    {
        [TestMethod]
        public void ShouldCacheIfEntityHasBeenRetrievedById()
        {
            const string CacheKey = "MDM.Location";

            // Given
            var mockMdmEntityService = new Mock<IMdmEntityService<Location>>();
            var mockConfigManager = new Mock<IConfigurationManager>();

            mockConfigManager.Setup(x => x.AppSettings).Returns(new NameValueCollection { { "CacheItemPolicy.Expiration." + CacheKey, "3500" } });
            var cachePolicyFactory = new AbsoluteCacheItemPolicyFactory(CacheKey, mockConfigManager.Object);
            var locationEntityService = new CachePolicyMdmEntityService<Location>(mockMdmEntityService.Object, cachePolicyFactory);
            var location = new Location
            {
                Identifiers = new NexusIdList { new NexusId { SystemName = "Nexus", Identifier = "1", IsNexusId = true } },
                Details = new LocationDetails { Name = "Blah" }
            };

            //Setup a context, i.e. calling once to retrieve location should cache an entity..
            mockMdmEntityService.Setup(x => x.Get(1, It.IsAny<DateTime?>())).Returns(
                new WebResponse<Location> { Code = HttpStatusCode.OK, IsValid = true, Message = location });

            var response = locationEntityService.Get(1);

            response.Code.Should().Be(HttpStatusCode.OK);
            response.Message.ToMdmId().Identifier.Should().Be("1");

            // When , we call second time..
            response = locationEntityService.Get(1);

            // Then 
            // It should not call mdm service again... should retrive form cache, so that verify...
            mockMdmEntityService.Verify(x => x.Get(1, It.IsAny<DateTime?>()), Times.Once());
            mockMdmEntityService.Verify(x => x.Get(It.IsAny<NexusId>(), It.IsAny<DateTime?>()), Times.Never());
            response.Code.Should().Be(HttpStatusCode.OK);
            response.Message.ToMdmId().Identifier.Should().Be("1");
        }

        [TestMethod]
        public void ShouldRetrieveFromCacheIfClientRequestByNexusIdMapping()
        {
            const string CacheKey = "MDM.Location";

            // Given
            var mockMdmEntityService = new Mock<IMdmEntityService<Location>>();
            var mockConfigManager = new Mock<IConfigurationManager>();

            mockConfigManager.Setup(x => x.AppSettings).Returns(new NameValueCollection { { "CacheItemPolicy.Expiration." + CacheKey, "3500" } });
            var cachePolicyFactory = new AbsoluteCacheItemPolicyFactory(CacheKey, mockConfigManager.Object);
            var locationEntityService = new CachePolicyMdmEntityService<Location>(mockMdmEntityService.Object, cachePolicyFactory);
            var nexusId = new NexusId { SystemName = "Nexus", Identifier = "1", IsNexusId = true };
            var location = new Location
            {
                Identifiers = new NexusIdList { nexusId },
                Details = new LocationDetails { Name = "Blah" }
            };

            //Setup a context, i.e. calling once to retrieve location should cache an entity..
            mockMdmEntityService.Setup(x => x.Get(1, It.IsAny<DateTime?>())).Returns(
                new WebResponse<Location> { Code = HttpStatusCode.OK, IsValid = true, Message = location });

            var response = locationEntityService.Get(nexusId);

            response.Code.Should().Be(HttpStatusCode.OK);
            response.Message.ToMdmId().Identifier.Should().Be(nexusId.Identifier);

            // When , we call second time..
            response = locationEntityService.Get(nexusId);

            // Then 
            // It should not call mdm service again... should retrive form cache, so that verify...
            mockMdmEntityService.Verify(x => x.Get(1, It.IsAny<DateTime?>()), Times.Once());
            mockMdmEntityService.Verify(x => x.Get(It.IsAny<NexusId>(), It.IsAny<DateTime?>()), Times.Never());
            response.Code.Should().Be(HttpStatusCode.OK);
            response.Message.ToMdmId().Identifier.Should().Be(nexusId.Identifier);
        }

        [TestMethod]
        public void ShouldRetrieveFromCacheIfClientRequestBySystemMapping()
        {
            const string CacheKey = "MDM.Location";

            // Given
            var mockMdmEntityService = new Mock<IMdmEntityService<Location>>();
            var mockConfigManager = new Mock<IConfigurationManager>();
            mockConfigManager.Setup(x => x.AppSettings).Returns(new NameValueCollection { { "CacheItemPolicy.Expiration." + CacheKey, "3500" } });

            var cachePolicyFactory = new AbsoluteCacheItemPolicyFactory(CacheKey, mockConfigManager.Object);
            var locationEntityService = new CachePolicyMdmEntityService<Location>(mockMdmEntityService.Object, cachePolicyFactory);
            var nexusId = new NexusId { SystemName = "Nexus", Identifier = "1", IsNexusId = true };
            var adcId = new NexusId { SystemName = "ADC", Identifier = "123", IsNexusId = false };
            var location = new Location
            {
                Identifiers = new NexusIdList { nexusId, adcId },
                Details = new LocationDetails { Name = "Blah" }
            };

            //Setup a context, i.e. calling once to retrieve location should cache an entity..
            mockMdmEntityService.Setup(x => x.Get(adcId, It.IsAny<DateTime?>())).Returns(
                new WebResponse<Location> { Code = HttpStatusCode.OK, IsValid = true, Message = location });

            var response = locationEntityService.Get(adcId);

            response.Code.Should().Be(HttpStatusCode.OK);
            response.Message.ToMdmId().Identifier.Should().Be(nexusId.Identifier);

            // When , we call second time..
            response = locationEntityService.Get(adcId);

            // Then 
            // It should not call mdm service again... should retrive form cache, so that verify...
            mockMdmEntityService.Verify(x => x.Get(adcId, It.IsAny<DateTime?>()), Times.Once());
            mockMdmEntityService.Verify(x => x.Get(It.IsAny<int>(), It.IsAny<DateTime?>()), Times.Never());
            response.Code.Should().Be(HttpStatusCode.OK);
            response.Message.ToMdmId().Identifier.Should().Be(nexusId.Identifier);
        }
    }
}