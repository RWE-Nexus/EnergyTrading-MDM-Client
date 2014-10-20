﻿namespace EnergyTrading.Mdm.Client.Services
{
    using System.Collections.Generic;

    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;
    using EnergyTrading.Mdm.Client.Extensions;

    public class ReferenceDataService : IReferenceDataService
    {
        private readonly string baseUri;
        private readonly IMessageRequester requester;

        public ReferenceDataService(string baseUri, IMessageRequester requester)
        {
            this.baseUri = baseUri;
            this.requester = requester;
        }

        public WebResponse<ReferenceDataList> List(string key)
        {
            var uri = string.Format("{0}/list/{1}", this.baseUri, key);
            var response = this.requester.Request<ReferenceDataList>(uri);
            response.LogResponse();
            return response;
        }

        public WebResponse<IList<ReferenceData>> Create(string key, IList<ReferenceData> entries)
        {
            var uri = string.Format("{0}/create/{1}", this.baseUri, key);
            var response = this.requester.Create(uri, entries);
            response.LogResponse();
            return response;
        }

        public WebResponse<IList<ReferenceData>> Delete(string key, IList<ReferenceData> entries)
        {
            var uri = string.Format("{0}/delete/{1}", this.baseUri, key);
            var response= this.requester.Delete<IList<ReferenceData>>(uri);
            response.LogResponse();
            return response;
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