using System;
using System.Configuration;

namespace EnergyTrading.Mdm.Client.Constants
{
   /// <summary>
   /// 
   /// </summary>
   public static class MdmConstants
    {
       public static readonly string MdmRequestHeaderName = "X-MDMREQ-HDR";
       public static readonly string MdmRequestSourceSystemName = "Mdm:SourceSystem";

       public static readonly bool LogResponse =
           Convert.ToBoolean(ConfigurationManager.AppSettings["LogResponse"] ?? "false");
    }
}
