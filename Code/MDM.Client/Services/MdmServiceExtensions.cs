namespace EnergyTrading.MDM.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;

    using EnergyTrading.MDM.Client.Extensions;
    using EnergyTrading.MDM.Client.WebClient;

    using EnergyTrading.Logging;
    using RWEST.Nexus.MDM.Contracts;

    public static class MdmServiceExtensions
    {
        /// <summary>
        /// Attempt multiple calls to the MDM service until we are successful or exhaust the retries.
        /// </summary>
        /// <param name="service">MDM service to use</param>
        /// <param name="func">Function to invoke against MDM</param>
        /// <param name="logger">Logger to use</param>
        /// <param name="retries">Number of retries, defaults to 5</param>
        /// <param name="sleep">Dwell time between retries, defaults to 100 ms</param>
        /// <returns>Result of the MDM action</returns>   
        public static WebResponse<T> Try<T>(this IMdmService service, ILogger logger, Func<WebResponse<T>> func, int retries = 5, int sleep = 100)
        {
            return service.Try(func, logger, retries, sleep);
        }

        /// <summary>
        /// Attempt multiple calls to the MDM service until we are successful or exhaust the retries.
        /// </summary>
        /// <param name="service">MDM service to use</param>
        /// <param name="func">Function to invoke against MDM</param>
        /// <param name="logger">Logger to use</param>
        /// <param name="retries">Number of retries, defaults to 5</param>
        /// <param name="sleep">Dwell time between retries, defaults to 100 ms</param>
        /// <returns>Result of the MDM action</returns>   
        public static WebResponse<T> Try<T>(this IMdmService service, Func<WebResponse<T>> func, ILogger logger, int retries = 5, int sleep = 100)
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
                        response.IsValid = true;
                    }
                }
                else
                {
                    isServiceUnvailable = false;
                }

                if (!response.IsValid)
                {
                    Thread.Sleep(sleep);
                    logger.Debug("Try_" + GetTypeName(typeof(T)) + ": " + response.Code);
                }
            }

            if (isServiceUnvailable)
            {
                logger.Error("Service Call to MDM Service has failed with the error: " + fault.Message);
            }

            return response;
        }

        /// <summary>
        /// Attempt multiple calls to the MDM service until we are successful or exhaust the retries.
        /// </summary>
        /// <param name="service">MDM service to use</param>
        /// <param name="func">Function to invoke against MDM</param>
        /// <param name="log">Action to perform on failure</param>
        /// <param name="retries">Number of retries, defaults to 5</param>
        /// <param name="sleep">Dwell time between retries, defaults to 100 ms</param>
        /// <returns>Result of the MDM action</returns>
        [Obsolete("Use override that takes ILogger")]
        public static WebResponse<T> Try<T>(this IMdmService service, Func<WebResponse<T>> func, Action<string> log, int retries = 5, int sleep = 100)
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
                        response.IsValid = true;
                    }
                }
                else
                {
                    isServiceUnvailable = false;
                }

                if (!response.IsValid)
                {
                    Thread.Sleep(sleep);
                    log("Try_" + GetTypeName(typeof(T)) + ": " + response.Code);
                }
            }

            if (isServiceUnvailable)
            {
                log("Service Call to MDM Service has failed with the error: " + fault.Message);
            }

            return response;  
        }

        /// <summary>
        /// Attempt multiple calls to the MDM service until we are successful or exhaust the retries.
        /// </summary>
        /// <typeparam name="T">Type we are returning</typeparam>
        /// <param name="service">MDM service to use</param>
        /// <param name="func">Function to invoke against MDM</param>
        /// <param name="logger">Action to perform on failure</param>
        /// <param name="retries">Number of retries, defaults to 5</param>
        /// <param name="sleep">Dwell time between retries, defaults to 100 ms</param>
        /// <returns>Result of the MDM action</returns>        
        public static WebResponse<T> TrySearch<T>(this IMdmService service, ILogger logger, Func<WebResponse<T>> func, int retries = 5, int sleep = 100)
        {
            return service.TrySearch(func, logger, retries, sleep);
        }

        /// <summary>
        /// Attempt multiple calls to the MDM service until we are successful or exhaust the retries.
        /// </summary>
        /// <typeparam name="T">Type we are returning</typeparam>
        /// <param name="service">MDM service to use</param>
        /// <param name="func">Function to invoke against MDM</param>
        /// <param name="logger">Action to perform on failure</param>
        /// <param name="retries">Number of retries, defaults to 5</param>
        /// <param name="sleep">Dwell time between retries, defaults to 100 ms</param>
        /// <returns>Result of the MDM action</returns>   
        public static WebResponse<T> TrySearch<T>(this IMdmService service, Func<WebResponse<T>> func, ILogger logger, int retries = 5, int sleep = 100)
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
                        response.IsValid = true;
                    }
                }
                else
                {
                    isServiceUnvailable = false;
                }

                if (!response.IsValid)
                {
                    logger.Debug("Try_" + GetTypeName(typeof(T)) + ": " + response.Code);
                    Thread.Sleep(sleep);
                }
            }

            if (isServiceUnvailable)
            {
                logger.Error("Service Call to MDM Service has failed with the error: " + fault.Message);
            }

            return response;
        }

        /// <summary>
        /// Attempt multiple calls to the MDM service until we are successful or exhaust the retries.
        /// </summary>
        /// <typeparam name="T">Type we are returning</typeparam>
        /// <param name="service">MDM service to use</param>
        /// <param name="func">Function to invoke against MDM</param>
        /// <param name="log">Action to perform on failure</param>
        /// <param name="retries">Number of retries, defaults to 5</param>
        /// <param name="sleep">Dwell time between retries, defaults to 100 ms</param>
        /// <returns>Result of the MDM action</returns> 
        [Obsolete("Use override that takes ILogger")]        
        public static WebResponse<T> TrySearch<T>(this IMdmService service, Func<WebResponse<T>> func, Action<string> log, int retries = 5, int sleep = 100)
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
                        response.IsValid = true;
                    }
                }
                else
                {
                    isServiceUnvailable = false;
                }

                if (!response.IsValid)
                {
                    log("Try_" + GetTypeName(typeof(T)) + ": " + response.Code);
                    Thread.Sleep(sleep);
                }
            }

            if (isServiceUnvailable)
            {
                log("Service Call to MDM Service has failed with the error: " + fault.Message);
            }

            return response;
        }

        private static string GetTypeName(Type type)
        {
            if (type.IsGenericType &&
                (type.GetGenericTypeDefinition() == typeof(List<>) ||
                type.GetGenericTypeDefinition() == typeof(IList<>) ||
                type.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                var genericArguments = type.GetGenericArguments();

                if (genericArguments.Any())
                {
                    return genericArguments.First().Name;
                }
            }

            return type.Name;
        }
    }
}