namespace EnergyTrading.MDM.Client.Services.Model
{
    using RWEST.Nexus.MDM.Contracts;

    public class ModelShipperCodeService : IMdmModelEntityService<ShipperCode, Client.Model.ShipperCode>
    {
        private readonly IMdmModelEntityService service;

        public ModelShipperCodeService(IMdmModelEntityService service)
        {
            this.service = service;
        }

        public Client.Model.ShipperCode Get(ShipperCode contract)
        {
            if (contract == null)
            {
                return null;
            }

            var model = new Client.Model.ShipperCode
            {
                Source = contract,
                Location = this.service.Location(contract.ToMdmKey(x => x.Details.Location)),
                Party = this.service.Party(contract.ToMdmKey(x => x.Details.Party)),
            };

            return model;
        }
    }
}