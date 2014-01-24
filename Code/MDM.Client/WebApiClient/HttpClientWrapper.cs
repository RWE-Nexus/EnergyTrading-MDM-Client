namespace EnergyTrading.MDM.Client.WebApiClient
{
    using System;
    using System.Net.Http;
    using System.Reflection;

    using EnergyTrading.Logging;

    /// <summary>
    /// Simple implementation of IHttpClient that uses the Microsoft ASP.Net Web API HttpClient
    /// </summary>
    public class HttpClientWrapper : IHttpClient
    {
        private readonly static ILogger Logger = LoggerFactory.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly HttpClient client;

        public HttpClientWrapper(string address)
        {
            var handler = new HttpClientHandler { UseDefaultCredentials = true };

            this.client = new HttpClient(handler)
            { 
                BaseAddress = new Uri(address),
            };
        }

        public HttpResponseMessage Get(string uri)
        {
            Logger.DebugFormat("Get: {0}", uri);
            return this.client.GetAsync(uri).Result;
        }

        public HttpResponseMessage Delete(string uri)
        {
            Logger.DebugFormat("Delete: {0}", uri);
            return this.client.DeleteAsync(uri).Result;
        }
      
        public void Dispose()
        {
            this.client.Dispose();
        }

        public HttpResponseMessage Post(string uri, HttpContent content)
        {
            Logger.DebugFormat("Post: {0}", uri);
            return this.client.PostAsync(uri, content).Result;
        }

        public void AddHeader(string key, string value)
        {
            Logger.DebugFormat("Key: {0} Value: {0}", key, value);
            this.client.DefaultRequestHeaders.Add(key, value);
        }
    }
}