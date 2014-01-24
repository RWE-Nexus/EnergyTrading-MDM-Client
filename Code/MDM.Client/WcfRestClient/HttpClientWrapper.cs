namespace EnergyTrading.MDM.Client.WcfRestClient
{
    using System.Net;
    using System.Reflection;

    using Microsoft.Http;

    using EnergyTrading.Logging;

    /// <summary>
    /// Simple implementation of IHttpClient that use the Microsoft HttpClient
    /// </summary>
    /// <remarks>
    /// Can fail under load due to TCP/IP port exhaustion - typical error message is
    /// Unable to connect to the remote server: Only one usage of each socket address (protocol/network address/port) is normally permitted 
    /// </remarks>
    public class HttpClientWrapper : IHttpClient
    {
        private readonly static ILogger Logger = LoggerFactory.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly HttpClient client;

        public HttpClientWrapper(string address)
        {
            this.client = new HttpClient(address)
            { 
                TransportSettings =
                {
                    Credentials = CredentialCache.DefaultNetworkCredentials
                } 
            };
        }

        public HttpResponseMessage Get(string uri)
        {
            Logger.DebugFormat("Get: {0}", uri);
            return this.client.Get(uri);
        }

        public HttpResponseMessage Delete(string uri)
        {
            Logger.DebugFormat("Delete: {0}", uri);
            return this.client.Delete(uri);
        }
      
        public void Dispose()
        {
            this.client.Dispose();
        }

        public HttpResponseMessage Post(string uri, HttpContent content)
        {
            Logger.DebugFormat("Post: {0}", uri);
            return this.client.Post(uri, content);
        }

        public void AddHeader(string key, string value)
        {
            Logger.DebugFormat("Key: {0} Value: {0}", key, value);
            this.client.DefaultHeaders.Add(key, value);
        }
    }
}