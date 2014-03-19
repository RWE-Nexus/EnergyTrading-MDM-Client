namespace EnergyTrading.Mdm.Client.Services
{
    using System;
    using System.Collections.Generic;

    using EnergyTrading.Contracts.Search;
    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;

    /// <summary>
    /// REST API for an MDM entity.
    /// </summary>
    /// <typeparam name="TContract"></typeparam>
    public interface IMdmEntityService<TContract>
         where TContract : IMdmEntity
    {
        /// <summary>
        /// Create a MDM entity.
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        WebResponse<TContract> Create(TContract contract);

        /// <summary>
        /// Create a mapping for a MDM entity.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        WebResponse<MdmId> CreateMapping(int id, MdmId identifier);

        /// <summary>
        /// Locate the cross-map for a MDM system.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="targetSystem"></param>
        /// <returns></returns>
        WebResponse<MappingResponse> CrossMap(MdmId identifier, string targetSystem);

        /// <summary>
        /// Locate the cross-map for a MDM system.
        /// </summary>
        /// <param name="sourceSystem"></param>
        /// <param name="identifier"></param>
        /// <param name="targetSystem"></param>
        /// <returns></returns>
        WebResponse<MappingResponse> CrossMap(string sourceSystem, string identifier, string targetSystem);

        /// <summary>
        /// Delete a mapping for a MDM entity.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="mappingId"></param>
        /// <returns></returns>
        WebResponse<TContract> DeleteMapping(int entityId, int mappingId);

        /// <summary>
        /// Get a MDM entity.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        WebResponse<TContract> Get(int id);

        /// <summary>
        /// Get full temporal history of a MDM entity.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        WebResponse<IList<TContract>> GetList(int id);

        /// <summary>
        /// Get a MDM entity.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="validAt"></param>
        /// <returns></returns>
        WebResponse<TContract> Get(int id, DateTime? validAt);

        /// <summary>
        /// Get a MDM entity.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        WebResponse<TContract> Get(MdmId identifier);

        /// <summary>
        /// Get a MDM entity.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="validAt"></param>
        /// <returns></returns>
        WebResponse<TContract> Get(MdmId identifier, DateTime? validAt);

        /// <summary>
        /// Get a mapping for a MDM entity.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        WebResponse<MdmId> GetMapping(int id, Predicate<MdmId> query);

        /// <summary>
        /// Invalidate the cache for a MDM entity.
        /// </summary>
        /// <param name="id"></param>
        void Invalidate(int id);

        /// <summary>
        /// Locate the mapping for a MDM entity.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="targetSystem"></param>
        /// <returns></returns>
        WebResponse<MdmId> Map(int id, string targetSystem);

        /// <summary>
        /// Perform a search for a MDM entity.
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        PagedWebResponse<IList<TContract>> Search(Search search);

        /// <summary>
        /// Update a MDM entity.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contract"></param>
        /// <returns></returns>
        WebResponse<TContract> Update(int id, TContract contract);

        /// <summary>
        /// Update a MDM entity.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contract"></param>
        /// <param name="etag"></param>
        /// <returns></returns>
        WebResponse<TContract> Update(int id, TContract contract, string etag);
    }
}
