﻿namespace EnergyTrading.Mdm.Client.WebApi.WebApiClient
{
    using System.Net;
    using System.Net.Http;

    /// <summary>
    /// Handles faults.
    /// </summary>
    public interface IFaultHandler
    {
        /// <summary>
        /// Process a response message for errors
        /// </summary>
        /// <param name="message">Response message we received.</param>
        /// <param name="httpStatusCode">Status code that we are expecting.</param>
        /// <returns>true if the message is ok to continue, false otherwise</returns>
        bool Handle(HttpResponseMessage message, HttpStatusCode httpStatusCode);
    }
}