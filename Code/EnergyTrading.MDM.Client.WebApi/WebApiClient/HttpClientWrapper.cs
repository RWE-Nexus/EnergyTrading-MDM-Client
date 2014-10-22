namespace EnergyTrading.Mdm.Client.WebApi.WebApiClient
{
    using System;
    using System.Net.Http;
    using System.Reflection;
    using EnergyTrading.Logging;
    using EnergyTrading.Mdm.Client.WebApi.Extensions;

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
            Logger.DebugFormat("HttpClientWrapper.Get: {0}", uri);
            var response= this.client.GetAsync(uri).Result;
            response.LogResponse();
            return response;
        }

        public HttpResponseMessage Delete(string uri)
        {
            Logger.DebugFormat("HttpClientWrapper.Delete: {0}", uri);
            var response= this.client.DeleteAsync(uri).Result;
            response.LogResponse();
            return response;
        }
      
        public void Dispose()
        {
            this.client.Dispose();
        }

        public HttpResponseMessage Post(string uri, HttpContent content)
        {
            Logger.DebugFormat("HttpClientWrapper.Post: {0}", uri);
            var response= this.client.PostAsync(uri, content).Result;
            response.LogResponse();
            return response;
        }

        public void AddHeader(string key, string value)
        {
            Logger.DebugFormat("HttpClientWrapper.AddHeader : Key: {0} Value: {0}", key, value);
            this.client.DefaultRequestHeaders.Add(key, value);
        }
    }
}