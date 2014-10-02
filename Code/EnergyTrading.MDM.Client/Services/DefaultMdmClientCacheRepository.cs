using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyTrading.Caching;
using EnergyTrading.Mdm.Contracts;
using Microsoft.Practices.Unity;

namespace EnergyTrading.Mdm.Client.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultMdmClientCacheRepository:IMdmClientCacheRepository
    {
        private ICacheRepository cacheRepository;

        public DefaultMdmClientCacheRepository(ICacheRepository cacheRepository)
        {
            this.cacheRepository = cacheRepository;
            RepositoryName = cacheRepository.GetType().Name;
        }

        /// <summary>
        /// 
        /// </summary>
        public string RepositoryName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public IMdmClientCacheService GetNamedCache(string cacheName) 
        {
            return new DefaultMdmClientCacheService(cacheRepository.GetNamedCache(cacheName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheName"></param>
        /// <returns></returns>
        public bool RemoveNamedCache(string cacheName)
        {
            return cacheRepository.RemoveNamedCache(cacheName);
        }

        public bool ClearNamedCache(string cacheName)
        {
            return cacheRepository.ClearNamedCache(cacheName);
        }
    }
}
