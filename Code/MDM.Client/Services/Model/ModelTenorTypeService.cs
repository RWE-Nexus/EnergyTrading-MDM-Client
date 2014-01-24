namespace EnergyTrading.MDM.Client.Services.Model
{
    using RWEST.Nexus.MDM.Contracts;

    public class ModelTenorTypeService : IMdmModelEntityService<TenorType, Client.Model.TenorType>
    {
        private readonly IMdmModelEntityService service;

        public ModelTenorTypeService(IMdmModelEntityService service)
        {
            this.service = service;
        }

        public Client.Model.TenorType Get(TenorType contract)
        {
            if (contract == null)
            {
                return null;
            }


            var model = new Client.Model.TenorType
            {
                Source = contract,
            };
            model.Tenors.AddRange(this.service.TenorTypeTenors(contract));

            return model;
        }
    }
}