using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnergyTrading.Mdm.Client.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        public static string GetCacheNameFromEntityName(this string entityName, uint version = 0)
        {
            return "Mdm." + entityName + (version > 0 ? "V" + version : string.Empty);
        }

        public static string GetCacheNameFromEntityType(this Type mdmEntity, uint version = 0)
        {
            return GetCacheNameFromEntityName(mdmEntity.Name, version);
        }

    }
}
