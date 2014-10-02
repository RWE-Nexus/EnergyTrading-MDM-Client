using System.Runtime.Caching;
using EnergyTrading.Caching;
using EnergyTrading.Mdm.Contracts;

namespace EnergyTrading.Mdm.Client.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContract"></typeparam>
    public interface IMdmClientCacheService:ICacheService  
    {
        TContract Get<TContract>(MdmId mdmId)  where TContract : class,IMdmEntity;
        TContract Get<TContract>(int identifier) where TContract : class,IMdmEntity;
        bool Remove(int mdmIdentifier);
        void Add<TContract>(TContract value, CacheItemPolicy policy = null) where TContract :class, IMdmEntity;
        void Add(MdmId value, int identifier, CacheItemPolicy policy = null);
    }
}
