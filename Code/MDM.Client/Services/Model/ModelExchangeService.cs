namespace EnergyTrading.MDM.Client.Services.Model
{
    using RWEST.Nexus.MDM.Contracts;

    public class ModelExchangeService : IMdmModelEntityService<Exchange, Client.Model.Exchange>
    {
        public Client.Model.Exchange Get(RWEST.Nexus.MDM.Contracts.Exchange contract)
        {
            if (contract == null)
            {
                return null;
            }

            var model = new Client.Model.Exchange
            {
                Source = contract,
                Name = contract.Details.Name
            };

            return model;
        }
    }
}
