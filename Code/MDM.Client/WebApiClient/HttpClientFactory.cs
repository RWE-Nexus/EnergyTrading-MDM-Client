namespace EnergyTrading.MDM.Client.WebApiClient
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public IHttpClient Create(string request)
        {
            return new HttpClientWrapper(request);
        }
    }
}