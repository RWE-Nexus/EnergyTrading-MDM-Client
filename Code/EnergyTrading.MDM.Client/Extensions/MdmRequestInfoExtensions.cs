using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnergyTrading.Mdm.Client.WebClient;
using EnergyTrading.Xml.Serialization;

namespace EnergyTrading.Mdm.Client.Extensions
{
    public static class MdmRequestInfoExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="encodedMdmRequestInfo"></param>
        /// <returns></returns>
        public static MdmRequestInfo Decode(this string encodedMdmRequestInfo)
        {
           return Encoding.UTF8.GetString(Convert.FromBase64String(encodedMdmRequestInfo))
                .DeserializeDataContractXmlString<MdmRequestInfo>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        public static string Encode(this MdmRequestInfo requestInfo)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(requestInfo.DataContractSerialize()));
        }

    }
}
