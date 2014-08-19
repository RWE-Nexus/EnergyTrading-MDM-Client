namespace EnergyTrading.Mdm.Client.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using EnergyTrading.Contracts.Atom;
    using EnergyTrading.Contracts.Search;
    using EnergyTrading.Mdm.Client.Services;
    using EnergyTrading.Mdm.Contracts;
    using EnergyTrading.Search;

    /// <summary>
    /// Extensions to make MDM searches easier.
    /// </summary>
    public static class SearchExtensions
    {
        /// <summary>
        /// Adds a search clause to the current criteria based on the MDM <see cref="EntityId" /> 
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="searchField"></param>
        /// <param name="entity"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static SearchCriteria AddMdmIdCriteria(this SearchCriteria criteria, string searchField, IMdmEntity entity, SearchCondition condition = SearchCondition.Equals)
        {
            var nexusId = entity.ToMdmKeyString();

            criteria.AddCriteria(searchField, condition, nexusId, true);

            return criteria;
        }

        /// <summary>
        /// Adds a search clause to the current criteria based on a MDM <see cref="EntityId" />
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="searchField"></param>
        /// <param name="entity"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static SearchCriteria AddEntityIdCriteria(this SearchCriteria criteria, string searchField, EntityId entity, SearchCondition condition = SearchCondition.Equals)
        {
            // null string translated to Is Null query in MDM service
            var nexusId = "null";

            // if entity is set we will by now have found its id by now.
            if (entity != null)
            {
                nexusId = entity.Identifier.Identifier;
            }

            criteria.AddCriteria(searchField, condition, nexusId, true);
            return criteria;
        }

        /// <summary>
        /// Create a mapping search.
        /// </summary>
        /// <param name="systemName">System name to use</param>
        /// <param name="identifier">Identifier to use</param>
        /// <returns>A new mapping search</returns>
        public static Search MappingSearch(this string systemName, string identifier = null)
        {
            var search = SearchBuilder.CreateSearch(isMappingSearch: true);
            search.SearchOptions.ResultsPerPage = null;
            search.SearchOptions.MultiPage = false;

            search.AddSearchCriteria(SearchCombinator.And)
                .AddCriteria("System.Name", SearchCondition.Equals, systemName);

            if (!string.IsNullOrEmpty(identifier))
            {
                search.SearchFields.Criterias[0].AddCriteria("Mapping", SearchCondition.Equals, identifier);
            }

            return search;
        }

        /// <summary>
        /// Perform a mapping search.
        /// </summary>
        /// <typeparam name="T">Type of entity to find</typeparam>
        /// <param name="service">MDM service to use</param>
        /// <param name="systemName">System name to use</param>
        /// <param name="identifier">Identifier to use</param>
        /// <returns>Result of the search.</returns>
        public static IList<T> MappingSearch<T>(this IMdmService service, string systemName, string identifier = null)
            where T : IMdmEntity
        {
            var search = systemName.MappingSearch(identifier);

            return service.Search<T>(search).HandleResponse();
        }

        /// <summary>
        /// Extract links for an MDM entity type.
        /// </summary>
        /// <typeparam name="T">Type to use</typeparam>
        /// <param name="links">Atom links to process</param>
        /// <returns></returns>
        public static IEnumerable<int> ReferencedIds<T>(this IEnumerable<Link> links)
        {
            var typeName = typeof(T).Name;
            var regex = string.Format("/{0}/([0-9]+)$", typeName);

            var typeLinks = links.Where(x => x.Type == typeName);

            var ids = new List<int>();
            foreach (var link in typeLinks)
            {
                var match = Regex.Match(link.Uri, regex);
                if (match.Success)
                {
                    ids.Add(int.Parse(match.Groups[1].Value));
                }
            }
            return ids;
        }

        /// <summary>
        /// Try to execute a search, detecting if the service is available.
        /// </summary>
        /// <typeparam name="T">Type contained in the response.</typeparam>
        /// <typeparam name="TU">Target type, may be same as <see typeref="T" /></typeparam>
        /// <param name="service">MDM service to use</param>
        /// <param name="search">Search to execute</param>
        /// <param name="func">Function to transform the response into the target.</param>
        /// <param name="results">Results of the search</param>
        /// <returns>true if the search worked, false otherwise.</returns>
        public static bool TrySearch<T, TU>(this IMdmModelEntityService service, Search search, Func<T, TU> func, out IList<TU> results)
            where T : IMdmEntity
        {
            var x = service.TrySearch(search, func);

            results = x.Item2;
            return x.Item1;
        }

        /// <summary>
        /// Try to execute a search, detecting if the service is available.
        /// </summary>
        /// <typeparam name="T">Type contained in the response.</typeparam>
        /// <typeparam name="TU">Target type, may be same as <see typeref="T" /></typeparam>        
        /// <param name="service">MDM service to use</param>
        /// <param name="search">Search to execute</param>
        /// <returns>true if the search worked, false otherwise.</returns>
        public static Tuple<bool, IList<TU>> TrySearch<T, TU>(this IMdmModelEntityService service, Search search, Func<T, TU> func)
            where T : IMdmEntity
        {
            var response = service.Search<T>(search);
            return response.Fault.IsServiceUnavailable()
                ? Tuple.Create(false, (IList<TU>)new List<TU>())
                : Tuple.Create(true, response.HandleResponse(func));
        }

        /// <summary>
        /// Try to execute a search, detecting if the service is available.
        /// </summary>
        /// <typeparam name="T">Type contained in the response.</typeparam>
        /// <param name="service">MDM service to use</param>
        /// <param name="search">Search to execute</param>
        /// <param name="results">Results of the search</param>
        /// <returns>true if the search worked, false otherwise.</returns>
        public static bool TrySearch<T>(this IMdmModelEntityService service, Search search, out IList<T> results)
            where T : IMdmEntity
        {
            var x = service.TrySearch<T>(search);

            results = x.Item2;
            return x.Item1;
        }

        /// <summary>
        /// Try to execute a search, detecting if the service is available.
        /// </summary>
        /// <typeparam name="T">Type contained in the response.</typeparam>
        /// <param name="service">MDM service to use</param>
        /// <param name="search">Search to execute</param>
        /// <param name="version">optional version of contract to use in Search (default = 0)</param>
        /// <returns>true if the search worked, false otherwise.</returns>
        public static Tuple<bool, IList<T>> TrySearch<T>(this IMdmModelEntityService service, Search search, uint version = 0)
            where T : IMdmEntity
        {
            var response = service.Search<T>(search, version);
            return response.Fault.IsServiceUnavailable() 
                ? Tuple.Create(false, (IList<T>) new List<T>()) 
                : Tuple.Create(true, response.HandleResponse());
        }
    }
}