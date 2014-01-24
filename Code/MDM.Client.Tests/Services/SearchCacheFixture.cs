namespace EnergyTrading.MDM.Client.Tests.Services
{
    using System.Collections.Generic;
    using System.Runtime.Caching;

    using EnergyTrading.Caching;
    using EnergyTrading.Contracts.Search;
    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.WebClient;
    using EnergyTrading.Search;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class SearchCacheFixture
    {
        private CachePolicyMdmEntityService<Dimension> cacheService;
        private Mock<IMdmEntityService<Dimension>> mdmService;
        private Mock<ICacheItemPolicyFactory> policyFactory;
        private PagedWebResponse<IList<Dimension>> searchResult;
        private CacheItemPolicy policy;
        
        [TestInitialize]
        public void Setup()
        {
            this.mdmService = new Mock<IMdmEntityService<Dimension>>();
            this.policyFactory = new Mock<ICacheItemPolicyFactory>();
            this.policy = new CacheItemPolicy();

            this.policyFactory.Setup(x => x.CreatePolicy()).Returns(this.policy);

            this.cacheService = new CachePolicyMdmEntityService<Dimension>(this.mdmService.Object, this.policyFactory.Object);           
        }

        [TestMethod]
        public void ServiceInvokedOnFirstCall()
        {
            // Arrange
            var search = SearchBuilder.CreateSearch();
            search.AddSearchCriteria(SearchCombinator.And)
                .AddCriteria("A", SearchCondition.Equals, "34", false);

            this.searchResult = new PagedWebResponse<IList<Dimension>>();
            this.mdmService.Setup(x => x.Search(search)).Returns(this.searchResult);

            // Act
            var result = this.cacheService.Search(search);

            // Assert
            Assert.AreEqual(this.searchResult, result);
            this.mdmService.Verify(x => x.Search(search), Times.AtMostOnce());
        }

        [TestMethod]
        public void CacheUsedOnSecondCall()
        {
            // Arrange
            var search = SearchBuilder.CreateSearch();
            search.AddSearchCriteria(SearchCombinator.And)
                .AddCriteria("A", SearchCondition.Equals, "34", false);

            this.searchResult = new PagedWebResponse<IList<Dimension>>();
            this.mdmService.Setup(x => x.Search(search)).Returns(this.searchResult);

            // Act
            this.cacheService.Search(search);
            var result = this.cacheService.Search(search);

            // Assert
            Assert.AreEqual(this.searchResult, result);
            this.mdmService.Verify(x => x.Search(search), Times.AtMostOnce());
        }

        [TestMethod]
        public void InvalidResponseIsReturned()
        {
            // Arrange
            var search = SearchBuilder.CreateSearch();
            search.AddSearchCriteria(SearchCombinator.And)
                .AddCriteria("A", SearchCondition.Equals, "34", false);

            this.searchResult = new PagedWebResponse<IList<Dimension>> { IsValid = false };
            this.mdmService.Setup(x => x.Search(search)).Returns(this.searchResult);

            // Act
            var result = this.cacheService.Search(search);

            // Assert
            Assert.AreEqual(this.searchResult, result);
        }

        [TestMethod]
        public void InvalidResponseIsReturnedAndCached()
        {
            // Arrange
            var search = SearchBuilder.CreateSearch();
            search.AddSearchCriteria(SearchCombinator.And)
                .AddCriteria("A", SearchCondition.Equals, "34", false);

            this.searchResult = new PagedWebResponse<IList<Dimension>> { IsValid = false };
            this.mdmService.Setup(x => x.Search(search)).Returns(this.searchResult);

            // Act
            this.cacheService.Search(search);
            var result = this.cacheService.Search(search);

            // Assert
            Assert.AreEqual(this.searchResult, result);
            this.mdmService.Verify(x => x.Search(search), Times.AtMostOnce());
        }
    }
}