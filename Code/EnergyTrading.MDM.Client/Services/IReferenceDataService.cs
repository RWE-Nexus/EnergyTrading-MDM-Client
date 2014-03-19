namespace EnergyTrading.Mdm.Client.Services
{
    using System.Collections.Generic;

    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;

    public interface IReferenceDataService
    {
        WebResponse<ReferenceDataList> List(string key);

        WebResponse<IList<ReferenceData>> Create(string key, IList<ReferenceData> entries);

        WebResponse<IList<ReferenceData>> Delete(string key, IList<ReferenceData> entries);

        //PagedWebResponse<IList<ReferenceData>> Search(Search search);
    }
}