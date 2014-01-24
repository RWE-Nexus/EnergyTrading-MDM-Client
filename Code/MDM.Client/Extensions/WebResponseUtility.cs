namespace EnergyTrading.MDM.Client.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading;

    using EnergyTrading.MDM.Client.WebClient;

    using EnergyTrading.Logging;
    using RWEST.Nexus.MDM.Contracts;

    public static class WebResponseUtility
    {
        private static readonly ILogger Logger = LoggerFactory.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        /// <summary>
        /// Get back a list of values from the response.
        /// </summary>
        /// <typeparam name="T">Type contained in the response.</typeparam>
        /// <param name="response">Response to process</param>
        /// <returns>Empty list if the response is invalid, otherwise the contents of the response.</returns>
        public static IList<T> HandleResponse<T>(this WebResponse<IList<T>> response)
        {
            if (response != null && response.IsValid)
            {
                return response.Message == null
                    ? new List<T>()
                    : response.Message.Select(x => x).ToList();
            }

            return new List<T>();
        }

        /// <summary>
        /// Get back a list of values from the response
        /// </summary>
        /// <typeparam name="T">Type contained in the response.</typeparam>
        /// <typeparam name="TU">Target type, may be same as <see typeref="T" /></typeparam>
        /// <param name="response">Response to process</param>
        /// <param name="func">Function to transform the response into the target.</param>
        /// <returns>Empty list if the response is invalid, otherwise the contents of the response.</returns>
        public static IList<TU> HandleResponse<T, TU>(this WebResponse<IList<T>> response, Func<T, TU> func)
        {
            if (response != null && response.IsValid)
            {
                return response.Message == null
                    ? new List<TU>()
                    : response.Message.Select(func).ToList();
            }

            return new List<TU>();
        }

        /// <summary>
        /// Determine whether a fault amounts to service unavailability.
        /// </summary>
        /// <param name="fault">Fault to check</param>
        /// <returns>true if the fault indicates service unavailability, false otherwise.</returns>
        public static bool IsServiceUnavailable(this Fault fault)
        {
            if (fault == null)
            {
                return false;
            }

            return fault.Message == "NotFound" ||
                   fault.Message == "ServiceUnavailable" ||
                   fault.Message.Contains("Unable to connect to the remote server");
        }

        /// <summary>
        /// Retry a web invocation until it works or we exhaust the retries.
        /// </summary>
        /// <typeparam name="T">Type we are trying to find</typeparam>
        /// <param name="func">Web function to invoke</param>
        /// <param name="retries">Number of retry attempts</param>
        /// <param name="sleep">Time to sleep between retries in milliseconds</param>
        /// <returns></returns>
        /// <remarks>Will also abort early if </remarks>
        public static T Retry<T>(this Func<WebResponse<T>> func, int retries = 3, int sleep = 100)
            where T : IMdmEntity
        {
            var response = new WebResponse<T> { IsValid = false };
            var i = 0;
            var isServiceUnvailable = false;
            Fault fault = null;

            while (!response.IsValid && i++ < retries)
            {
                response = func.Invoke();

                if (response.Fault != null)
                {
                    fault = response.Fault;

                    // Logic to handle the Service Unavailable scenarios
                    if (fault.IsServiceUnavailable())
                    {
                        isServiceUnvailable = true;
                    }
                    else if (response.Code == HttpStatusCode.NotFound)
                    {
                        // NB NotFound is not an error under normal circumstances
                        return default(T);
                    }
                }
                else
                {
                    isServiceUnvailable = false;
                }

                if (!response.IsValid)
                {
                    Thread.Sleep(sleep);
                }
            }

            if (isServiceUnvailable)
            {
                Logger.Error("Service Call to MDM Service has failed with the error: " + fault.Message);
            }

            return response.IsValid ? response.Message : default(T);
        }
    }
}