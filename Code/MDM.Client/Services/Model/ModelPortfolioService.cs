namespace EnergyTrading.MDM.Client.Services.Model
{
    using RWEST.Nexus.MDM.Contracts;

    public class ModelPortfolioService : IMdmModelEntityService<Portfolio, Client.Model.Portfolio>
    {
        private readonly IMdmModelEntityService service;

        public ModelPortfolioService(IMdmModelEntityService service)
        {
            this.service = service;
        }

        public Client.Model.Portfolio Get(RWEST.Nexus.MDM.Contracts.Portfolio contract)
        {
            if (contract == null)
            {
                return null;
            }

            var model = new Client.Model.Portfolio
            {
                Source = contract,
                BusinessUnit = this.service.PartyRole(contract, x => x.Details.BusinessUnit)
            };

            return model;
        }
    }
}
