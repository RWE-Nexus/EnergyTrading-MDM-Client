namespace EnergyTrading.Mdm.Client.WebApi.WebApiClient
{
    using System.Net;
    using System.Net.Http;

    /// <summary>
    /// Standard fault handler.
    /// </summary>
    public class StandardFaultHandler : IFaultHandler
    {
        /// <summary>
        /// Process the message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool Handle(HttpResponseMessage message, HttpStatusCode code)
        {
            switch (code)
            {
                case HttpStatusCode.Unauthorized:
                default:
                    return code == message.StatusCode;
            }
        }
    }
}