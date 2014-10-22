namespace EnergyTrading.Mdm.Client.WebApi.Extensions
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Reflection;
    using System.Text;
    using EnergyTrading.Logging;
    using EnergyTrading.Mdm.Client.Constants;

    public static class HttpResponseMessageExtension
    {
        private readonly static ILogger Logger = LoggerFactory.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static string GetResponseToLog(HttpResponseMessage response)
        {
            if (!MdmConstants.LogResponse)
                return string.Format("Status Code : {0}\nReason Phrase : {1}", response.StatusCode,
                    response.ReasonPhrase);

            var message = GetContent(response);

            return string.Format("Status Code : {0}\nReason Phrase : {1}\nContent : {2}",
                response.StatusCode,
                response.ReasonPhrase,
                message);
        }

        private static string GetContent(HttpResponseMessage response)
        {
            var message = string.Empty;
            var stream = new MemoryStream();
            try
            {
                var readTask = response.Content.CopyToAsync(stream).ContinueWith(task => task);
                readTask.Wait();
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    message = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Logger.DebugFormat("HttpResponseMessageExtension.LogResponse : Exception while reading content - {0}",
                    ex.Message);
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
            return message;
        }

        public static void LogResponse(this HttpResponseMessage response)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Http Response received : {0}", GetResponseToLog(response));
            }
        }
    }
}