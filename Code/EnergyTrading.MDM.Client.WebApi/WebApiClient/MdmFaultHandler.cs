namespace EnergyTrading.Mdm.Client.WebApi.WebApiClient
{
    using System;
    using System.Net;
    using System.Net.Http;

    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;

    public class MdmFaultHandler : IFaultHandler
    {
        public bool Handle(HttpResponseMessage message, HttpStatusCode httpStatusCode)
        {
            if (message.StatusCode != httpStatusCode)
            {
                throw new MdmFaultException(this.GetFault(message));
            }

            return true;
        }

        private Fault GetFault(HttpResponseMessage response)
        {
            Fault fault;
            try
            {
                fault = response.Content.ReadAsAsync<Fault>().Result;
            }
            catch (Exception)
            {
                fault = new Fault { Message = response.StatusCode.ToString() };
            }

            return fault;
        }
    }
}