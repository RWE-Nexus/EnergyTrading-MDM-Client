namespace EnergyTrading.Mdm.Client.Services
{
    using System;
    using System.Collections.Generic;
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
            var response = this.Create<MdmId, MappingResponse>(uri, mapping);
            if (response.IsValid)
            {
                return new WebResponse<MdmId>
                {
                    Code = HttpStatusCode.OK,
                    Message = response.Message.Mappings[0]
                };
            }

            return new WebResponse<MdmId>
            {
                IsValid = false,
                Code = response.Code,
                Fault = response.Fault
            };
        }

        public WebResponse<TContract> DeleteMapping(int entityId, int mappingId)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("DeleteMapping<{0}>: {1} - {2}", typeof(TContract).Name, entityId, mappingId);
            }

            var uri = string.Format(this.deleteMappingUri, entityId, mappingId);

            var response = this.requester.Delete<TContract>(uri);

            if (response.IsValid)
            {
                return new WebResponse<TContract>
                {
                    Code = HttpStatusCode.OK,
                    IsValid = true
                };
            }

            return new WebResponse<TContract>
            {
                Code = response.Code,
                IsValid = false,
                Fault = response.Fault
            };
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
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Create<{0}>", typeof(TContract).Name);
            }
            var response = this.Create(this.BaseUri, contract);
            if (response.IsValid)
            {
                this.ProcessContract(response);
            }

            return response;
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

            return this.CrossMap(identifier.SystemName, identifier.Identifier, targetSystem);
        }

        public WebResponse<MappingResponse> CrossMap(string sourceSystem, string identifier, string targetSystem)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("CrossMap<{0}>: {1} {2} {3}", typeof(TContract).Name, sourceSystem, identifier, targetSystem);
            }
            return this.requester.Request<MappingResponse>(string.Format(this.crossMapUri, sourceSystem, identifier, targetSystem));
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
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Update<{0}>: {1} {2}", typeof(TContract).Name, id, etag);
            }
            var response = this.requester.Update(string.Format(this.entityUri, id), etag, contract);
            if (!response.IsValid)
            {
                return response;
            }

            var queryString = string.Empty;
            if (contract.MdmSystemData != null && contract.MdmSystemData.StartDate.HasValue)
            {
                queryString = string.Format(this.validAtParam, contract.MdmSystemData.StartDate.Value.ToString(DateFormatString));
            }

            var r2 = this.requester.Request<TContract>(response.Location + queryString);
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
            return response;
        }

        private WebResponse<TMessage> Create<TMessage>(string uri, TMessage message) where TMessage : class
        {
            return this.Create<TMessage, TMessage>(uri, message);
        }

        private WebResponse<TResponse> Create<TMessage, TResponse>(string uri, TMessage message) where TResponse : class
        {
            var request = this.requester.Create(uri, message);
            return request.IsValid ? 
                   this.requester.Request<TResponse>(request.Location) : 
                   new WebResponse<TResponse> { Code = request.Code, IsValid = false, Fault = request.Fault };
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
    }
}