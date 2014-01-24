namespace EnergyTrading.MDM.Client.WebClient
{
    using System;

    /// <summary>
    /// Paged response to a web request.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedWebResponse<T> : WebResponse<T>
    {
        /// <summary>
        /// Gets or sets the URI to the next page.
        /// </summary>
        public Uri NextPage { get; set; }
    }
}