namespace EnergyTrading.Mdm.Client.WebClient
{
    using System;
    using System.Net;

    using EnergyTrading.Mdm.Contracts;

    /// <summary>
    /// Response to a web request
    /// </summary>
    /// <typeparam name="T">Type of the expected result.</typeparam>
    public class WebResponse<T>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="WebResponse{T}" /> class.
        /// </summary>
        public WebResponse()
        {
            this.IsValid = true;
        }

        /// <summary>
        /// Status code returned by the call
        /// </summary>
        public HttpStatusCode Code { get; set; }

        /// <summary>
        /// Is the response valid
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// The body of the response
        /// </summary>
        public T Message { get; set; }

        /// <summary>
        /// The actual fault the response is invalid.
        /// </summary>
        public Fault Fault { get; set; }

        /// <summary>
        /// The ETag supplied with the response
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Expiry time of the response
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// The returned location header
        /// </summary>
        public string Location { get; set; }
    }
}