namespace EnergyTrading.MDM.Client.WebApiClient
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Reflection;
    using System.ServiceModel.Syndication;
    using System.Xml;

    using EnergyTrading.MDM.Client.WebClient;

    using Microsoft.Practices.Unity;

    using EnergyTrading.Contracts.Search;
    using EnergyTrading.Extensions;
    using EnergyTrading.Logging;
    using RWEST.Nexus.MDM.Contracts;

    /// <summary>
    /// Implements a IMessageRequester using IHttpClientFactory.
    /// </summary>
    public class MessageRequester : IMessageRequester
    {
        private readonly static ILogger Logger = LoggerFactory.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IHttpClientFactory httpClientFactory;
        private readonly IFaultHandler faultHandler;

        // Used whilst we agree on this approach and then we can get rid of this constructor and each client can choose there own fault handling
        [InjectionConstructor]
        public MessageRequester(IHttpClientFactory httpClientFactory) : this(httpClientFactory, new StandardFaultHandler())
        {
        }

        public MessageRequester(IHttpClientFactory httpClientFactory, IFaultHandler faultHandler)
        {
            this.httpClientFactory = httpClientFactory;
            this.faultHandler = faultHandler;
        }

        /// <copydocfrom cref="IMessageRequester.Create{T}" />
        public WebResponse<TMessage> Create<TMessage>(string uri, TMessage message)
        {
            Logger.DebugFormat("Start: {0}", uri);
            var webResponse = new WebResponse<TMessage>();

            this.ResponseHandler(webResponse, uri, client =>
            {
                var content = new ObjectContent<TMessage>(message, new XmlMediaTypeFormatter());
                using (var response = client.Post(uri, content))
                {
                    this.PopulateWebResponse(webResponse, response, HttpStatusCode.Created);

                    if (webResponse.IsValid)
                    {
                        webResponse.Location = this.Location(uri, response);
                    }
                }
            });

            Logger.DebugFormat("Finish: {0}", uri);
            return webResponse;
        }

        /// <copydocfrom cref="IMessageRequester.Delete{T}" />
        public WebResponse<TMessage> Delete<TMessage>(string uri)
        {
            Logger.DebugFormat("Start: {0}", uri);
            var webResponse = new WebResponse<TMessage>();

            this.ResponseHandler(webResponse, uri, client =>
            {
                using (var response = client.Delete(uri))
                {
                    this.PopulateWebResponse(webResponse, response, HttpStatusCode.OK);
                }
            });

            Logger.DebugFormat("Finish: {0}", uri); 
            return webResponse;
        }

        /// <copydocfrom cref="IMessageRequester.Request{T}" />
        public WebResponse<TMessage> Request<TMessage>(string uri)
        {
            Logger.DebugFormat("Start: {0}", uri);
            var webResponse = new WebResponse<TMessage>();

            this.ResponseHandler(webResponse, uri, client =>
            {
                using (var response = client.Get(uri))
                {
                    this.PopulateWebResponse(webResponse, response, HttpStatusCode.OK);

                    if (webResponse.IsValid)
                    {
                        // Ok, get the other bits that should be there.
                        webResponse.Tag = (response.Headers.ETag == null) ? null : response.Headers.ETag.Tag;
                        webResponse.Message = response.Content.ReadAsAsync<TMessage>().Result;
                    }
                }
            });

            Logger.DebugFormat("Finish: {0}", uri);
            return webResponse;
        }

        /// <copydocfrom cref="IMessageRequester.Search{T}" />
        public PagedWebResponse<IList<TContract>> Search<TContract>(string uri, Search message)
        {
            Logger.DebugFormat("Start: {0}", uri);

            var result = new PagedWebResponse<IList<TContract>>
            {
                Message = new List<TContract>(0)
            };

            this.ResponseHandler(
                result,
                uri,
                client =>
                    {
                        var content = new ObjectContent<Search>(message, new XmlMediaTypeFormatter());
                        using (var response = client.Post(uri, content))
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore };
                                Stream stream = null;
                                var readTask = response.Content.ReadAsStreamAsync().ContinueWith(task =>
                                    {
                                        if (task.Exception != null)
                                        {
                                            result.Code = HttpStatusCode.InternalServerError;
                                            result.IsValid = false;
                                            result.Fault = new Fault { Message = "Exception reading response stream : " + task.Exception.Message };
                                        }
                                        else
                                        {
                                            stream = task.Result;
                                        }
                                        return task;
                                    });
                                readTask.Wait();
                                if (stream != null)
                                {
                                    var reader = XmlReader.Create(stream, settings);

                                    var feed = SyndicationFeed.Load(reader);
                                    if (feed == null)
                                    {
                                        result.Code = HttpStatusCode.InternalServerError;
                                    }
                                    else
                                    {
                                        result.Message = feed.Items.Select(syndicationItem => (XmlSyndicationContent)syndicationItem.Content).Select(syndic => syndic.ReadContent<TContract>()).ToList();
                                        result.Code = HttpStatusCode.OK;

                                        var nextPageLink = feed.Links.FirstOrDefault(syndicationLink => syndicationLink.RelationshipType == "next-results");
                                        result.NextPage = nextPageLink == null ? null : nextPageLink.Uri;
                                    }
                                }
                            }
                            else
                            {
                                result.Code = response.StatusCode;

                                if (response.StatusCode == HttpStatusCode.NotFound)
                                {
                                    result.IsValid = false;
                                    result.Message = new List<TContract>();
                                    return;
                                }

                                result.IsValid = false;
                                result.Fault = this.GetFault(response);
                            }
                        }
                    });

            Logger.DebugFormat("Finish: {0}", uri);
            return result;
        }

        /// <copydocfrom cref="IMessageRequester.Update{T}" />
        public WebResponse<TMessage> Update<TMessage>(string uri, string etag, TMessage message)
        {
            Logger.DebugFormat("Start: {0}", uri);

            var webResponse = new WebResponse<TMessage>();

            this.ResponseHandler(webResponse, uri, client =>
            {
                client.AddHeader("If-Match", etag);

                var content = new ObjectContent<TMessage>(message, new XmlMediaTypeFormatter());
                using (var response = client.Post(uri, content))
                {
                    this.PopulateWebResponse(webResponse, response, HttpStatusCode.NoContent);

                    if (webResponse.IsValid)
                    {
                        webResponse.Location = this.Location(uri, response);
                    }
                }
            });

            Logger.DebugFormat("Finish: {0}", uri);
            return webResponse;
        }

        private void ResponseHandler<T>(WebResponse<T> response, string uri, Action<IHttpClient> action)
        {
            try
            {
                using (var client = this.httpClientFactory.Create(uri))
                {
                    action(client);
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                response.Code = HttpStatusCode.InternalServerError;
                response.IsValid = false;
                response.Fault = new Fault { Message = ex.AllExceptionMessages() };
            }
        }

        private void PopulateWebResponse<T>(WebResponse<T> webResponse, HttpResponseMessage httpResponse, HttpStatusCode validCode)
        {
            webResponse.Code = httpResponse.StatusCode;
            webResponse.IsValid = this.faultHandler.Handle(httpResponse, validCode);          
            if (!webResponse.IsValid)
            {
                webResponse.Fault = this.GetFault(httpResponse);
            }
        }

        private string Location(string uri, HttpResponseMessage httpResponse)
        {
            var srcUri = new Uri(uri);
            return srcUri.GetLeftPart(UriPartial.Authority) + "/" + httpResponse.Headers.Location;  
        }

        private Fault GetFault(HttpResponseMessage response)
        {
            Fault fault;
            try
            {
                fault = response.Content.ReadAsAsync<Fault>().Result;
            }
            catch (Exception)
            {
                fault = new Fault { Message = response.StatusCode.ToString() };
            }

            return fault;
        }
    }
}