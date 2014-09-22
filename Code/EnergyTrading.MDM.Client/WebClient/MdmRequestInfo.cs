using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace EnergyTrading.Mdm.Client.WebClient
{
   /// <summary>
   /// This call would be serialized into http header of MDM call and would be used to identify the 
   /// requester when MDM publishes notification
   /// </summary>
   [DataContract]
   public class MdmRequestInfo
    {
       /// <summary>
       /// Each call to MDM would be associated with a unique id to identify the MDM call.
       /// </summary>
       [DataMember]
       public string RequestId { get; set; }
       /// <summary>
       /// Contains identifier to identify the caller
       /// </summary>
       [DataMember]
       public string SourceSystem { get; set; }
    }
}
