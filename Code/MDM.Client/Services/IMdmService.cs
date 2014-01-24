namespace EnergyTrading.MDM.Client.Services
{
    using System;
    using System.Collections.Generic;

    using EnergyTrading.MDM.Client.WebClient;

    using EnergyTrading.Contracts.Search;
    using RWEST.Nexus.MDM.Contracts;

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
        /// <returns></returns>
        WebResponse<TContract> Create<TContract>(TContract contract)
            where TContract : IMdmEntity;

        /// <summary>
        /// Create a mapping from an entity to a NexusId
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="id"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        WebResponse<NexusId> CreateMapping<TContract>(int id, NexusId identifier)
            where TContract : IMdmEntity;

        /// <summary>
        /// Create a mapping from an entity to a NexusId
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="entityId"></param>
        /// <param name="mappingId"></param>
        /// <returns></returns>
        WebResponse<TContract> DeleteMapping<TContract>(int entityId, int mappingId)
            where TContract : IMdmEntity;

        /// <summary>
        /// Get an entity.
        /// </summary>
        /// <typeparam name="TContract">Type of contract to find</typeparam>
        /// <param name="id">Identifier to use</param>
        /// <returns></returns>
        WebResponse<TContract> Get<TContract>(int id)
            where TContract : IMdmEntity;

        /// <summary>
        /// Gets the full historical list for a particular entity (useful for temporal entities).
        /// </summary>
        /// <typeparam name="TContract">Type of contract to find</typeparam>
        /// <param name="id">Identifier to use</param>
        /// <returns></returns>
        WebResponse<IList<TContract>> GetList<TContract>(int id)
            where TContract : IMdmEntity;

        /// <summary>
        /// Get an entity with an as of date.
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="id"></param>
        /// <param name="asof"></param>
        /// <returns></returns>
        WebResponse<TContract> Get<TContract>(int id, DateTime? asof)
            where TContract : IMdmEntity;

        /// <summary>
        /// Get an entity from an identifier
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="identifier"></param>
        /// <returns></returns>
        WebResponse<TContract> Get<TContract>(NexusId identifier)
            where TContract : IMdmEntity;

        /// <summary>
        /// Get an entity from an identifier with an as of date.
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="identifier"></param>
        /// <param name="asof"></param>
        /// <returns></returns>
        WebResponse<TContract> Get<TContract>(NexusId identifier, DateTime? asof)
            where TContract : IMdmEntity;

        /// <summary>
        /// Get a mapping that matches a predicate
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="id"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        WebResponse<NexusId> GetMapping<TContract>(int id, Predicate<NexusId> query)
            where TContract : IMdmEntity;

        /// <summary>
        /// Map from an MDM entity to a target system
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="id"></param>
        /// <param name="targetSystem"></param>
        /// <returns></returns>
        WebResponse<NexusId> Map<TContract>(int id, string targetSystem)
            where TContract : IMdmEntity;

        /// <summary>
        /// Map from a NexusId to another system
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="targetSystem"></param>
        /// <returns></returns>
        WebResponse<MappingResponse> CrossMap<TContract>(NexusId identifier, string targetSystem)
            where TContract : IMdmEntity;

        /// <summary>
        /// Map from one system to another.
        /// </summary>
        /// <param name="sourceSystem"></param>
        /// <param name="identifier"></param>
        /// <param name="targetSystem"></param>
        /// <returns></returns>
        WebResponse<MappingResponse> CrossMap<TContract>(string sourceSystem, string identifier, string targetSystem)
            where TContract : IMdmEntity;

        /// <summary>
        /// Invalidate an entity in the cache
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="id"></param>
        void Invalidate<TContract>(int id)
            where TContract : IMdmEntity;

        /// <summary>
        /// Search for entities
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="search"></param>
        /// <returns></returns>
        PagedWebResponse<IList<TContract>> Search<TContract>(Search search)
            where TContract : IMdmEntity;

        /// <summary>
        /// Update an entity including etag for checking the update is still valid
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <param name="etag"></param>
        /// <returns></returns>
        WebResponse<TContract> Update<TContract>(int id, TContract entity, string etag)
            where TContract : IMdmEntity;
    }
}