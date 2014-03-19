namespace EnergyTrading.Mdm.Client.Tests.Services
{
    using System;
    using System.Collections.Specialized;
    using System.Net;

    using EnergyTrading.Caching;
    using EnergyTrading.Configuration;
    using EnergyTrading.Mdm.Client.Services;
    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SharpTestsEx;

    [TestClass]
    public class CachePolicyMdmEntityServiceFixture
    {
        [TestMethod]
        public void ShouldCacheIfEntityHasBeenRetrievedById()
        {
            const string CacheKey = "MDM.SourceSystem";

            // Given
            var mockMdmEntityService = new Mock<IMdmEntityService<SourceSystem>>();
            var mockConfigManager = new Mock<IConfigurationManager>();

            mockConfigManager.Setup(x => x.AppSettings).Returns(new NameValueCollection { { "CacheItemPolicy.Expiration." + CacheKey, "3500" } });
            var cachePolicyFactory = new AbsoluteCacheItemPolicyFactory(CacheKey, mockConfigManager.Object);
            var locationEntityService = new CachePolicyMdmEntityService<SourceSystem>(mockMdmEntityService.Object, cachePolicyFactory);
            var location = new SourceSystem
            {
                Identifiers = new MdmIdList { new MdmId { SystemName = "Nexus", Identifier = "1", IsMdmId = true } },
                Details = new SourceSystemDetails { Name = "Blah" }
            };

            //Setup a context, i.e. calling once to retrieve location should cache an entity..
            mockMdmEntityService.Setup(x => x.Get(1, It.IsAny<DateTime?>())).Returns(
                new WebResponse<SourceSystem> { Code = HttpStatusCode.OK, IsValid = true, Message = location });

            var response = locationEntityService.Get(1);

            response.Code.Should().Be(HttpStatusCode.OK);
            response.Message.ToMdmId().Identifier.Should().Be("1");

            // When , we call second time..
            response = locationEntityService.Get(1);

            // Then 
            // It should not call mdm service again... should retrive form cache, so that verify...
            mockMdmEntityService.Verify(x => x.Get(1, It.IsAny<DateTime?>()), Times.Once());
            mockMdmEntityService.Verify(x => x.Get(It.IsAny<MdmId>(), It.IsAny<DateTime?>()), Times.Never());
            response.Code.Should().Be(HttpStatusCode.OK);
            response.Message.ToMdmId().Identifier.Should().Be("1");
        }

        [TestMethod]
        public void ShouldRetrieveFromCacheIfClientRequestByNexusIdMapping()
        {
            const string CacheKey = "MDM.SourceSystem";

            // Given
            var mockMdmEntityService = new Mock<IMdmEntityService<SourceSystem>>();
            var mockConfigManager = new Mock<IConfigurationManager>();

            mockConfigManager.Setup(x => x.AppSettings).Returns(new NameValueCollection { { "CacheItemPolicy.Expiration." + CacheKey, "3500" } });
            var cachePolicyFactory = new AbsoluteCacheItemPolicyFactory(CacheKey, mockConfigManager.Object);
            var locationEntityService = new CachePolicyMdmEntityService<SourceSystem>(mockMdmEntityService.Object, cachePolicyFactory);
            var nexusId = new MdmId { SystemName = "Nexus", Identifier = "1", IsMdmId = true };
            var location = new SourceSystem
            {
                Identifiers = new MdmIdList { nexusId },
                Details = new SourceSystemDetails { Name = "Blah" }
            };

            //Setup a context, i.e. calling once to retrieve location should cache an entity..
            mockMdmEntityService.Setup(x => x.Get(1, It.IsAny<DateTime?>())).Returns(
                new WebResponse<SourceSystem> { Code = HttpStatusCode.OK, IsValid = true, Message = location });

            var response = locationEntityService.Get(nexusId);

            response.Code.Should().Be(HttpStatusCode.OK);
            response.Message.ToMdmId().Identifier.Should().Be(nexusId.Identifier);

            // When , we call second time..
            response = locationEntityService.Get(nexusId);

            // Then 
            // It should not call mdm service again... should retrive form cache, so that verify...
            mockMdmEntityService.Verify(x => x.Get(1, It.IsAny<DateTime?>()), Times.Once());
            mockMdmEntityService.Verify(x => x.Get(It.IsAny<MdmId>(), It.IsAny<DateTime?>()), Times.Never());
            response.Code.Should().Be(HttpStatusCode.OK);
            response.Message.ToMdmId().Identifier.Should().Be(nexusId.Identifier);
        }

        [TestMethod]
        public void ShouldRetrieveFromCacheIfClientRequestBySystemMapping()
        {
            const string CacheKey = "MDM.SourceSystem";

            // Given
            var mockMdmEntityService = new Mock<IMdmEntityService<SourceSystem>>();
            var mockConfigManager = new Mock<IConfigurationManager>();
            mockConfigManager.Setup(x => x.AppSettings).Returns(new NameValueCollection { { "CacheItemPolicy.Expiration." + CacheKey, "3500" } });

            var cachePolicyFactory = new AbsoluteCacheItemPolicyFactory(CacheKey, mockConfigManager.Object);
            var locationEntityService = new CachePolicyMdmEntityService<SourceSystem>(mockMdmEntityService.Object, cachePolicyFactory);
            var nexusId = new MdmId { SystemName = "Nexus", Identifier = "1", IsMdmId = true };
            var adcId = new MdmId { SystemName = "ADC", Identifier = "123", IsMdmId = false };
            var location = new SourceSystem
            {
                Identifiers = new MdmIdList { nexusId, adcId },
                Details = new SourceSystemDetails { Name = "Blah" }
            };

            //Setup a context, i.e. calling once to retrieve location should cache an entity..
            mockMdmEntityService.Setup(x => x.Get(adcId, It.IsAny<DateTime?>())).Returns(
                new WebResponse<SourceSystem> { Code = HttpStatusCode.OK, IsValid = true, Message = location });

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