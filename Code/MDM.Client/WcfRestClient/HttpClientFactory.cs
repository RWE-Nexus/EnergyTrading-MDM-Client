namespace EnergyTrading.MDM.Client.WcfRestClient
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public IHttpClient Create(string request)
        {
            return new HttpClientWrapper(request);
        }
    }
}