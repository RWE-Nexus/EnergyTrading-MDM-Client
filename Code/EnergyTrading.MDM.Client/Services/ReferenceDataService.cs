namespace EnergyTrading.Mdm.Client.Services
{
    using System.Collections.Generic;
    using System.Reflection;
    using EnergyTrading.Logging;
    using EnergyTrading.Mdm.Client.Extensions;
    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;
    
    public class ReferenceDataService : IReferenceDataService
    {
        private readonly string baseUri;
        private readonly IMessageRequester requester;
        private static readonly ILogger Logger = LoggerFactory.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public ReferenceDataService(string baseUri, IMessageRequester requester)
        {
            this.baseUri = baseUri;
            this.requester = requester;
        }

        public WebResponse<ReferenceDataList> List(string key)
        {
            try
            {
                Logger.DebugFormat("Start : ReferenceDataService.List - {0}", key);
                var uri = string.Format("{0}/list/{1}", this.baseUri, key);
                var response = this.requester.Request<ReferenceDataList>(uri);
                response.LogResponse();
                return response;

            }
            finally
            {
                Logger.Debug("Stop : ReferenceDataService.List");
            }
        }

        public WebResponse<IList<ReferenceData>> Create(string key, IList<ReferenceData> entries)
        {
            try
            {
                Logger.DebugFormat("Start : ReferenceDataService.Create - {0}", key);
                var uri = string.Format("{0}/create/{1}", this.baseUri, key);
                var response = this.requester.Create(uri, entries);
                response.LogResponse();
                return response;
            }
            finally
            {
                Logger.Debug("Stop : ReferenceDataService.Create");
            }
        }

        public WebResponse<IList<ReferenceData>> Delete(string key, IList<ReferenceData> entries)
        {
            try
            {
                Logger.DebugFormat("Start : ReferenceDataService.Delete {0}", key);
                var uri = string.Format("{0}/delete/{1}", this.baseUri, key);
                var response = this.requester.Delete<IList<ReferenceData>>(uri);
                response.LogResponse();
                return response;
            }
            finally
            {
                Logger.Debug("Stop : ReferenceDataService.Delete");
            }
        }

        //public PagedWebResponse<IList<ReferenceData>> Search(Search search)
        //{
        //    var uri = string.Format("{0}/search", baseUri);
        //    return requester.Search<ReferenceData>(uri, search);
        //    //var uri = string.Format("{0}/list/{1}", baseUri, search.SearchFields.Criterias.Count == 0 ? "{}" : search.SearchFields.Criterias[0].Criteria[0].ComparisonValue);
        //    //return requester.Request<IList<ReferenceData>>(uri);
        //}
    }
}