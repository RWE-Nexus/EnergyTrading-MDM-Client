namespace EnergyTrading.MDM.Client.WebClient
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;

    using Microsoft.Http;

    using RWEST.Nexus.MDM.Contracts;

    public class NexusFaultHandler : IFaultHandler
    {
        public bool Handle(HttpResponseMessage message, HttpStatusCode httpStatusCode)
        {
            if (message.StatusCode != httpStatusCode)
            {
                throw new NexusFaultException(this.GetFault(message));
            }

            return true;
        }

        private Fault GetFault(HttpResponseMessage response)
        {
            Fault fault;
            try
            {
                fault = response.Content.ReadAsDataContract<Fault>();
            }
            catch (Exception)
            {
                fault = new Fault() { Message = response.StatusCode.ToString() };
            }

            return fault;
        }
    }
}