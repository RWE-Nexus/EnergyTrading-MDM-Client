using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using EnergyTrading.Caching;
using EnergyTrading.Mdm.Contracts;

namespace EnergyTrading.Mdm.Client.Services
{
    /// <summary>
    /// This class wraps ICacheService and implements Mdm specific caching Apis
    /// </summary>
    public class DefaultMdmClientCacheService : IMdmClientCacheService
    {
        private readonly ICacheService cacheService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheService"></param>
        public DefaultMdmClientCacheService(ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mdmId"></param>
        /// <returns></returns>
        public TContract Get<TContract>(MdmId mdmId) where TContract : class,IMdmEntity
        {
            var entity = default(TContract);

            if (mdmId == null) return entity;

            if (mdmId.IsMdmId)
            {
                int id;
                if (int.TryParse(mdmId.Identifier, out id))
                {
                    return Get<TContract>(id);
                }
            }

            var identifier = cacheService.Get<int?>(CreateCacheKeyFromMdmId(mdmId));

            if (identifier.HasValue)
            {
                entity = Get<TContract>(identifier.Value);
                //If mapping doesnt belong to entity (may be mapping got removed) then clear it from cache
                if (entity == null || !entity.Identifiers.Any(a => a.Identifier == mdmId.Identifier && a.SystemName == mdmId.SystemName))
                {
                    cacheService.Remove(CreateCacheKeyFromMdmId(mdmId));
                    entity = null;
                }
            }

            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public TContract Get<TContract>(int identifier) where TContract : class,IMdmEntity
        {
            var entity = cacheService.Get<TContract>(identifier.ToString(CultureInfo.InvariantCulture));

            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mdmIdentifier"></param>
        /// <returns></returns>
        public bool Remove(int mdmIdentifier)
        {
            var cacheKey = CreateCacheKeyFromIdentifierForMappings(mdmIdentifier);
            var mappingList = cacheService.Get<MdmIdList>(cacheKey);

            if (mappingList != null)
            {
                //Remove Mapping to Identifier cache items
                foreach (var map in mappingList)
                {
                    cacheService.Remove(CreateCacheKeyFromMdmId(map));
                }
            }

            //Remove Identifier to mapping list cache item
            cacheService.Remove(cacheKey);

            return cacheService.Remove(mdmIdentifier.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="policy"></param>
        /// <typeparam name="TContract"></typeparam>
        public void Add<TContract>(TContract entity, CacheItemPolicy policy = null) where TContract : class,IMdmEntity
        {
            if (entity == null)
            {
                return;
            }
            AddMappingsToCache(entity, policy);
            cacheService.Add(entity.ToMdmKey().ToString(CultureInfo.InvariantCulture), entity, policy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mdmId"></param>
        /// <param name="identifier"></param>
        /// <param name="policy"></param>
        public void Add(MdmId mdmId, int identifier, CacheItemPolicy policy = null)
        {
            if (mdmId == null)
            {
                return;
            }

            cacheService.Add(CreateCacheKeyFromMdmId(mdmId), new int?(identifier));
        }

        private void AddMappingsToCache<TContract>(TContract entity, CacheItemPolicy policy) where TContract : class,IMdmEntity
        {
            var mdmIdentifier = entity.ToMdmId();
            var identifier = entity.ToMdmKey();

            foreach (var mdmId in entity.Identifiers.Where(a => !a.Equals(mdmIdentifier)))
            {
                Add(mdmId, identifier, policy);
            }

            cacheService.Add(CreateCacheKeyFromIdentifierForMappings(entity.ToMdmKey()), entity.Identifiers, policy);
        }

        private static string CreateCacheKeyFromMdmId(MdmId mdmId)
        {
            return string.Format("Mapping_{0}_{1}", mdmId.SystemName, mdmId.Identifier);
        }

        private static string CreateCacheKeyFromIdentifierForMappings(int identifier)
        {
            return string.Format("Entity_Mappings_{0}", identifier);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            return !string.IsNullOrWhiteSpace(key) && cacheService.Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="policy"></param>
        /// <typeparam name="T"></typeparam>
        public void Add<T>(string key, T value, CacheItemPolicy policy = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            cacheService.Add(key, value, policy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return string.IsNullOrWhiteSpace(key) ? default (T) : cacheService.Get<T>(key);
        }
    }
}
