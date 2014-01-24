namespace EnergyTrading.MDM.Client.WebApiClient
{
    /// <summary>
    /// Creates HTTP client instances.
    /// </summary>
    public interface IHttpClientFactory
    {
        /// <summary>
        /// Create a <see cref="IHttpClient" /> for a request.
        /// </summary>
        /// <param name="request">Request to use.</param>
        /// <returns>An <see cref="IHttpClient" /> instance.</returns>
        IHttpClient Create(string request);
    }
}