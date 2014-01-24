namespace EnergyTrading.MDM.Client.WcfRestClient
{
    using System;

    using Microsoft.Http;

    /// <summary>
    /// Simple HTTP client.
    /// </summary>
    public interface IHttpClient : IDisposable
    {
        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <param name="uri">URI to use</param>
        /// <returns>Response from the client.</returns>
        HttpResponseMessage Get(string uri);

        /// <summary>
        /// Deletes the resource.
        /// </summary>
        /// <param name="uri">URI to use</param>
        /// <returns>Response from the client.</returns>
        HttpResponseMessage Delete(string uri);

        /// <summary>
        /// Post the resource.
        /// </summary>
        /// <param name="uri">URI to use</param>
        /// <param name="content">Content to use</param>
        /// <returns>Response from the client.</returns>
        HttpResponseMessage Post(string uri, HttpContent content);

        /// <summary>
        /// Add a header to the current client.
        /// </summary>
        /// <param name="key">Key to use</param>
        /// <param name="value">Value to use</param>
        void AddHeader(string key, string value);
    }
}