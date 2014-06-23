namespace EnergyTrading.Mdm.Client.Services
{
    using System;
    using System.Collections.Generic;

    using EnergyTrading.Contracts.Search;
    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;

    /// <summary>
    /// Generic service for finding and creating MDM entities.
    /// </summary>
    public interface IMdmService
    {
        /// <summary>
        /// Create a new instance of TContract
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="contract"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        WebResponse<TContract> Create<TContract>(TContract contract, uint version = 0)
            where TContract : IMdmEntity;

        /// <summary>
        /// Create a mapping from an entity to a MdmId
        /// </summary>
        /// <typeparam name="TContract">
        /// </typeparam>
        /// <param name="id">
        /// </param>
        /// <param name="identifier">
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// </returns>
        WebResponse<MdmId> CreateMapping<TContract>(int id, MdmId identifier, uint version = 0)
            where TContract : IMdmEntity;

        /// <summary>
        /// Create a mapping from an entity to a MdmId
        /// </summary>
        /// <typeparam name="TContract">
        /// </typeparam>
        /// <param name="entityId">
        /// </param>
        /// <param name="mappingId">
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// </returns>
        WebResponse<TContract> DeleteMapping<TContract>(int entityId, int mappingId, uint version = 0)
            where TContract : IMdmEntity;

        /// <summary>
        /// Get an entity.
        /// </summary>
        /// <typeparam name="TContract">
        /// Type of contract to find
        /// </typeparam>
        /// <param name="id">
        /// Identifier to use
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// </returns>
        WebResponse<TContract> Get<TContract>(int id, uint version = 0)
            where TContract : IMdmEntity;

        /// <summary>
        /// Gets the full historical list for a particular entity (useful for temporal entities).
        /// </summary>
        /// <typeparam name="TContract">
        /// Type of contract to find
        /// </typeparam>
        /// <param name="id">
        /// Identifier to use
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// </returns>
        WebResponse<IList<TContract>> GetList<TContract>(int id, uint version = 0)
            where TContract : IMdmEntity;

        /// <summary>
        /// Get an entity with an as of date.
        /// </summary>
        /// <typeparam name="TContract">
        /// </typeparam>
        /// <param name="id">
        /// </param>
        /// <param name="asof">
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// </returns>
        WebResponse<TContract> Get<TContract>(int id, DateTime? asof, uint version = 0)
            where TContract : IMdmEntity;

        /// <summary>
        /// Get an entity from an identifier
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="identifier"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        WebResponse<TContract> Get<TContract>(MdmId identifier, uint version = 0)
            where TContract : IMdmEntity;

        /// <summary>
        /// Get an entity from an identifier with an as of date.
        /// </summary>
        /// <typeparam name="TContract">
        /// </typeparam>
        /// <param name="identifier">
        /// </param>
        /// <param name="asof">
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// </returns>
        WebResponse<TContract> Get<TContract>(MdmId identifier, DateTime? asof, uint version = 0)
            where TContract : IMdmEntity;

        /// <summary>
        /// Get a mapping that matches a predicate
        /// </summary>
        /// <typeparam name="TContract">
        /// </typeparam>
        /// <param name="id">
        /// </param>
        /// <param name="query">
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// </returns>
        WebResponse<MdmId> GetMapping<TContract>(int id, Predicate<MdmId> query, uint version = 0)
            where TContract : IMdmEntity;

        /// <summary>
        /// Map from an MDM entity to a target system
        /// </summary>
        /// <typeparam name="TContract">
        /// </typeparam>
        /// <param name="id">
        /// </param>
        /// <param name="targetSystem">
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// </returns>
        WebResponse<MdmId> Map<TContract>(int id, string targetSystem, uint version = 0)
            where TContract : IMdmEntity;

        /// <summary>
        /// Map from a MdmId to another system
        /// </summary>
        /// <typeparam name="TContract">
        /// </typeparam>
        /// <param name="identifier">
        /// </param>
        /// <param name="targetSystem">
        /// </param>
        /// <param name="version">
        /// </param>
        /// <returns>
        /// </returns>
        WebResponse<MappingResponse> CrossMap<TContract>(MdmId identifier, string targetSystem, uint version = 0)
            where TContract : IMdmEntity;

        /// <summary>
        /// Map from one system to another.
        /// </summary>
        /// <typeparam name="TContract">
        /// </typeparam>
        /// <param name="sourceSystem">
        /// </param>
        /// <param name="identifier">
        /// </param>
        /// <param name="targetSystem">
        /// </param>
        /// <param name="version">
        /// </param>
        /// <returns>
        /// </returns>
        WebResponse<MappingResponse> CrossMap<TContract>(string sourceSystem, string identifier, string targetSystem, uint version = 0)
            where TContract : IMdmEntity;

        /// <summary>
        /// Invalidate an entity in the cache
        /// </summary>
        /// <typeparam name="TContract">
        /// </typeparam>
        /// <param name="id">
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        void Invalidate<TContract>(int id, uint version = 0)
            where TContract : IMdmEntity;

        /// <summary>
        /// Search for entities
        /// </summary>
        /// <typeparam name="TContract">
        /// </typeparam>
        /// <param name="search">
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// </returns>
        PagedWebResponse<IList<TContract>> Search<TContract>(Search search, uint version = 0)
            where TContract : IMdmEntity;

        /// <summary>
        /// Update an entity including etag for checking the update is still valid
        /// </summary>
        /// <typeparam name="TContract">
        /// </typeparam>
        /// <param name="id">
        /// </param>
        /// <param name="entity">
        /// </param>
        /// <param name="etag">
        /// </param>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// </returns>
        WebResponse<TContract> Update<TContract>(int id, TContract entity, string etag, uint version = 0)
            where TContract : IMdmEntity;
    }
}