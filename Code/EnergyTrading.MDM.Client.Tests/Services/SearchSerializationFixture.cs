namespace EnergyTrading.Mdm.Client.Tests.Services
{
    using EnergyTrading.Contracts.Search;
    using EnergyTrading.Search;
    using EnergyTrading.Xml.Serialization;

    using NUnit.Framework;

    /// <summary>
    /// Check Search serialization as we want to rely on it in caching
    /// </summary>
    [TestFixture]
    public class SearchSerializationFixture
    {
        [Test]
        public void SameContentHasSameSerialization()
        {
            var searchA = SearchBuilder.CreateSearch();
            searchA.AddSearchCriteria(SearchCombinator.And)
                .AddCriteria("A", SearchCondition.Equals, "34", true)
                .AddCriteria("B", SearchCondition.Equals, "Test");

            var searchB = SearchBuilder.CreateSearch();
            searchB.AddSearchCriteria(SearchCombinator.And)
                .AddCriteria("A", SearchCondition.Equals, "34", true)
                .AddCriteria("B", SearchCondition.Equals, "Test");

            var expected = searchA.DataContractSerialize();
            var candidate = searchB.DataContractSerialize();

            Assert.AreEqual(expected, candidate);
        }

        [Test]
        public void DifferContentHasDifferentSerialization()
        {
            var searchA = SearchBuilder.CreateSearch();
            searchA.AddSearchCriteria(SearchCombinator.And)
                .AddCriteria("A", SearchCondition.Equals, "34", true)
                .AddCriteria("B", SearchCondition.Equals, "Test");

            var searchB = SearchBuilder.CreateSearch();
            searchB.AddSearchCriteria(SearchCombinator.And)
                .AddCriteria("A", SearchCondition.Equals, "34", false)
                .AddCriteria("B", SearchCondition.Equals, "Test");

            var expected = searchA.DataContractSerialize();
            var candidate = searchB.DataContractSerialize();

            Assert.AreNotEqual(expected, candidate);
        }
    }
}