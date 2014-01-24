namespace EnergyTrading.MDM.Client.Services.Model
{
    using RWEST.Nexus.MDM.Contracts;

    public class ModelCurveService : IMdmModelEntityService<Curve, Client.Model.Curve>
    {
        private readonly IMdmModelEntityService service;

        public ModelCurveService(IMdmModelEntityService service)
        {
            this.service = service;
        }

        public Client.Model.Curve Get(Curve contract)
        {
            if (contract == null)
            {
                return null;
            }

            var model = new Client.Model.Curve
            {
                Source = contract,
                Commodity = this.service.ModelCommodity(contract, x => x.Details.Commodity),
                Location = this.service.Location(contract.ToMdmKey(x => x.Details.Location)),
                Originator = this.service.Party(contract.ToMdmKey(x => x.Details.Originator))
            };

            return model;
        }
    }
}