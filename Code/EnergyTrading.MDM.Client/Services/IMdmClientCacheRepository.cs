using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyTrading.Caching;
using EnergyTrading.Mdm.Contracts;

namespace EnergyTrading.Mdm.Client.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMdmClientCacheRepository
    {
        string RepositoryName { get; }
        IMdmClientCacheService GetNamedCache(string cacheName);
        bool RemoveNamedCache(string cacheName);
        bool ClearNamedCache(string cacheName);
    }
}
