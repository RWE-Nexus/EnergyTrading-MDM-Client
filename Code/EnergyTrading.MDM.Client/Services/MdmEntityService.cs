namespace EnergyTrading.Mdm.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Web;

    using EnergyTrading.Contracts.Search;
    using EnergyTrading.Logging;
    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;

    public class MdmEntityService<TContract> : IMdmEntityService<TContract>
        where TContract : class, IMdmEntity
    {
        private readonly static ILogger Logger = LoggerFactory.GetLogger(typeof(MdmEntityService<TContract>));
        private readonly Dictionary<int, string> etags;
        private readonly IMessageRequester requester;
        private readonly string entityUri;
        private readonly string entityListUri;
        private readonly string mapUri;
        private readonly string crossMapUri;
        private readonly string mappingUri;
        private readonly string deleteMappingUri;
        private readonly string searchUri;
        private readonly string validAtParam;

        private const string DateFormatString = "yyyy-MM-dd'T'HH:mm:ss.fffffffZ";

        public MdmEntityService(string baseUri, IMessageRequester requester)
        {
            this.BaseUri = baseUri;
            this.entityUri = baseUri + "/{0}";
            this.mappingUri = this.entityUri + "/mapping";
            this.entityListUri = this.entityUri + "/list";
            this.deleteMappingUri = this.mappingUri + "/{1}";
            this.mapUri = baseUri + "/map?" + QueryConstants.SourceSystem + "={0}&" + QueryConstants.MappingValue + "={1}";
            this.crossMapUri = baseUri + "/crossmap?" + QueryConstants.SourceSystem + "={0}&" + QueryConstants.MappingValue + "={1}&" + QueryConstants.DestinationSystem + "={2}";
            this.searchUri = baseUri + "/search";
            this.validAtParam = "?" + QueryConstants.ValidAt + "={0}";

            this.requester = requester;
            this.etags = new Dictionary<int, string>();
        }

        public int Count
        {
            get { return 0; }
        }

        public int MappingCount
        {
            get { return 0; }
        }

        protected string BaseUri { get; set; }

        public void Clear()
        {
            this.etags.Clear();
        }

        public WebResponse<MdmId> CreateMapping(int id, MdmId identifier)
        {
            return CreateMapping(id, identifier, null);
        }

        public WebResponse<TContract> DeleteMapping(int entityId, int mappingId)
        {
            return DeleteMapping(entityId, mappingId, null);
        }

        public WebResponse<TContract> Get(int id)
        {
            // NB We push null here so that the service inteprets now and any requests can be cached upstream
            return this.Get(id, null);
        }

        public WebResponse<IList<TContract>> GetList(int id)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("GetList<{0}>: {1} - {2}", typeof(TContract).Name, id);
            }

            return this.requester.Request<IList<TContract>>(string.Format(this.entityListUri, id));
        }

        public WebResponse<TContract> Get(int id, DateTime? validAt)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Get<{0}>: {1} - {2}", typeof(TContract).Name, id, validAt);
            }
            return this.AcquireEntity(id, validAt);
        }

        public WebResponse<TContract> Get(MdmId identifier)
        {
            // NB We push null here so that the service inteprets now and any requests can be cached upstream
            return this.Get(identifier, null);
        }

        public WebResponse<TContract> Get(MdmId identifier, DateTime? validAt)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Get<{0}>: {1} {2}", typeof(TContract).Name, identifier, validAt);
            }
            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            if (identifier.IsMdmId || identifier.SystemName == SourceSystemNames.Nexus)
            {
                int id;
                if (int.TryParse(identifier.Identifier, out id))
                {
                    return this.Get(id, validAt);
                }

                throw new ArgumentException("Invalid Nexus identifier: {0}.", identifier.ToString());
            }

            return this.AcquireEntity(identifier, validAt);
        }

        public WebResponse<TContract> Create(TContract contract)
        {
            return Create(contract, null);
        }

        public WebResponse<MdmId> GetMapping(int id, Predicate<MdmId> query)
        {
            var response = this.Get(id);
            if (response.IsValid)
            {
                var nexusId = response.Message.Identifiers.FirstOrDefault(mapping => query(mapping));
                if (nexusId != null)
                {
                    return new WebResponse<MdmId>
                    {
                        Code = HttpStatusCode.OK,
                        Message = nexusId
                    };
                }

                // TODO: Add fault for "No mapping to system x"
                response.Code = HttpStatusCode.NotFound;
            }

            LogResponse(id, response, null, "GetMapping");

            return new WebResponse<MdmId>
            {
                Code = response.Code,
                IsValid = false,
                Fault = response.Fault
            };
        }

        public void Invalidate(int id)
        {
            string value;
            if (!this.etags.TryGetValue(id, out value))
            {
                return;
            }

            this.etags.Remove(id);
        }

        public WebResponse<MappingResponse> CrossMap(MdmId identifier, string targetSystem)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            var response = this.CrossMap(identifier.SystemName, identifier.Identifier, targetSystem);

            if (!response.IsValid)
            {
                LogResponse(response,
                    response.Code == HttpStatusCode.NotFound
                        ? "CrossMap<{0}> - Could not find mapping for {1} - {2}"
                        : "CrossMap<{0}> - {1} - {2}", typeof (TContract).Name,
                    identifier.SystemName, identifier.Identifier);
            }

            return response;
        }

        public WebResponse<MappingResponse> CrossMap(string sourceSystem, string identifier, string targetSystem)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("CrossMap<{0}>: {1} {2} {3}", typeof(TContract).Name, sourceSystem, identifier, targetSystem);
            }

            var response = this.requester.Request<MappingResponse>(string.Format(this.crossMapUri, sourceSystem, identifier, targetSystem));

            if (!response.IsValid)
            {
                LogResponse(response,
                    response.Code == HttpStatusCode.NotFound
                        ? "CrossMap<{0}> - Could not find mapping for {1} - {2}"
                        : "CrossMap<{0}> - {1} - {2}", typeof (TContract).Name,
                    sourceSystem, identifier);
            }

            return response;
        }

        public WebResponse<MdmId> Map(int id, string targetSystem)
        {
            return this.GetMapping(id, ident => string.Equals(ident.SystemName, targetSystem, StringComparison.InvariantCultureIgnoreCase));
        }

        public PagedWebResponse<IList<TContract>> Search(Search search)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Search<{0}>", typeof(TContract).Name);
            }
            return this.requester.Search<TContract>(this.searchUri, search);
        }

        public WebResponse<TContract> Update(int id, TContract contract)
        {
            return this.Update(id, contract, this.etags[id]);
        }

        public WebResponse<TContract> Update(int id, TContract contract, string etag)
        {
            return Update(id,contract,etag,null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        public WebResponse<TContract> Create(TContract contract, MdmRequestInfo requestInfo)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Create<{0}>", typeof(TContract).Name);
            }

            var response = this.Create(this.BaseUri, contract, requestInfo);

            if (response.IsValid)
            {
                this.ProcessContract(response);
            }
            else
            {
                if (contract.Identifiers != null && contract.Identifiers.Any())
                {
                    LogResponse(response, "Create<{0}> - {1}", typeof(TContract).Name, contract.Identifiers.First().Identifier);
                }
                else
                {
                    LogResponse(response, "Create<{0}>", typeof(TContract).Name);
                }
            }
            return response;
        }

        public WebResponse<MdmId> CreateMapping(int id, MdmId identifier, MdmRequestInfo requestInfo)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("CreateMapping<{0}>: {1} - {2}", typeof(TContract).Name, id, identifier);
            }
            var mapping = new Mapping
            {
                // TODO: Flesh out the rest
                SystemName = identifier.SystemName,
                Identifier = identifier.Identifier,
                DefaultReverseInd = identifier.DefaultReverseInd,
            };

            var uri = string.Format(this.mappingUri, id);
            var response = this.Create<MdmId, MappingResponse>(uri, mapping, requestInfo);
            if (response.IsValid)
            {
                return new WebResponse<MdmId>
                {
                    Code = HttpStatusCode.OK,
                    Message = response.Message.Mappings[0],
                    RequestId = response.RequestId
                };
            }

            return new WebResponse<MdmId>
            {
                IsValid = false,
                Code = response.Code,
                Fault = response.Fault,
                RequestId = response.RequestId
            };
        }

        public WebResponse<TContract> DeleteMapping(int entityId, int mappingId, MdmRequestInfo requestInfo)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("DeleteMapping<{0}>: {1} - {2}", typeof(TContract).Name, entityId, mappingId);
            }

            var uri = string.Format(this.deleteMappingUri, entityId, mappingId);

            var response = this.requester.Delete<TContract>(uri, requestInfo);

            if (response.IsValid)
            {
                return new WebResponse<TContract>
                {
                    Code = HttpStatusCode.OK,
                    IsValid = true,
                    RequestId = response.RequestId
                };
            }

            LogResponse(mappingId, response, null, "DeleteMapping");

            return new WebResponse<TContract>
            {
                Code = response.Code,
                IsValid = false,
                Fault = response.Fault,
                RequestId = response.RequestId
            };
        }

        public WebResponse<TContract> Update(int id, TContract contract, MdmRequestInfo requestInfo)
        {
            return Update(id, contract, this.etags[id], requestInfo);
        }

        public WebResponse<TContract> Update(int id, TContract contract, string etag, MdmRequestInfo requestInfo)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Update<{0}>: {1} {2}", typeof(TContract).Name, id, etag);
            }

            var response = this.requester.Update(string.Format(this.entityUri, id), etag, contract, requestInfo);

            if (!response.IsValid)
            {
                LogResponse(id, response, null, "Update");
                return response;
            }

            var queryString = string.Empty;
            if (contract.MdmSystemData != null && contract.MdmSystemData.StartDate.HasValue)
            {
                queryString = string.Format(this.validAtParam, contract.MdmSystemData.StartDate.Value.ToString(DateFormatString));
            }

            var r2 = this.requester.Request<TContract>(response.Location + queryString);

            r2.RequestId = response.RequestId;

            // TODO: Should this just be a IsValid check?
            if (r2.Code == HttpStatusCode.OK)
            {
                this.ProcessContract(r2);
            }

            return r2;
        }

        private WebResponse<TContract> AcquireEntity(int id, DateTime? validAt)
        {
            var uri = string.Format(this.entityUri, id);
            if (validAt.HasValue)
            {
                uri += string.Format(this.validAtParam, validAt.Value.ToString(DateFormatString));
            }

            var response = this.GetContract(uri);

            LogResponse(id, response, validAt, null);

            return response;
        }

        private WebResponse<TContract> AcquireEntity(MdmId sourceIdentifier, DateTime? validAt)
        {
            var uri = string.Format(this.mapUri, UrlEncode(sourceIdentifier.SystemName), UrlEncode(sourceIdentifier.Identifier));
            if (validAt.HasValue)
            {
                uri += string.Format(this.validAtParam, validAt.Value.ToString(DateFormatString));
            }

            var response = this.GetContract(uri);

            LogResponse(sourceIdentifier.Identifier, response, validAt, null);

            return response;
        }

        private WebResponse<TMessage> Create<TMessage>(string uri, TMessage message, MdmRequestInfo requestInfo) where TMessage : class
        {
            return this.Create<TMessage, TMessage>(uri, message,requestInfo);
        }

        private WebResponse<TResponse> Create<TMessage, TResponse>(string uri, TMessage message, MdmRequestInfo requestInfo) where TResponse : class
        {
            var request = this.requester.Create(uri, message, requestInfo);
            var response = new WebResponse<TResponse> {Code = request.Code, IsValid = false, RequestId = request.RequestId, Fault = request.Fault};
            
            if (!request.IsValid) return response;

            response = this.requester.Request<TResponse>(request.Location);
            response.RequestId = request.RequestId;

            return response;
        }

        private WebResponse<TContract> GetContract(string uri)
        {
            var entity = this.Request<TContract>(uri);
            if (entity.IsValid)
            {
                // Only process if we found data!
                this.ProcessContract(entity);
            }

            return entity;
        }

        private void ProcessContract(WebResponse<TContract> reponse)
        {
            var entity = reponse.Message;
            var id = entity.ToMdmKey();

            this.etags[id] = reponse.Tag;
        }

        private WebResponse<TMessage> Request<TMessage>(string uri)
            where TMessage : class
        {
            return this.requester.Request<TMessage>(uri);
        }

        private static string UrlEncode(string value)
        {
            return HttpUtility.UrlEncode(value);
        }

        private static void LogResponse<T>(WebResponse<T> response, string format, params object[] parameters)
        {
            const string MessageFormat = "{0} {1}";

            if (response.IsValid)
            {
                return;
            }

            if (response.Fault == null)
            {
                Logger.InfoFormat(MessageFormat, string.Format(format, parameters), response.Code);
            }
            else
            {
                Logger.InfoFormat(MessageFormat, string.Format(format, parameters), response.Fault.Message);
            }
        }

        private static void LogResponse(string id, WebResponse<TContract> response, DateTime? validAt, string verb = "GET")
        {
            LogResponse(response, "{0}<{1}> - Id {2} {3}", verb, typeof(TContract).Name, id, validAt);
        }

        private static void LogResponse(int id, WebResponse<TContract> response, DateTime? validAt, string verb = "GET")
        {
            LogResponse(id.ToString(CultureInfo.InvariantCulture), response, validAt, verb);
        }
    }
}