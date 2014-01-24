namespace EnergyTrading.MDM.Client.Services.Model
{
    using RWEST.Nexus.MDM.Contracts;

    public class ModelBrokerService : IMdmModelEntityService<Broker, Client.Model.Broker>
    {
        public Client.Model.Broker Get(RWEST.Nexus.MDM.Contracts.Broker contract)
        {
            if (contract == null)
            {
                return null;
            }

            var model = new Client.Model.Broker
            {
                Source = contract,
                Name = contract.Details.Name
            };

            return model;
        }
    }
}