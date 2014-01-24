namespace MDM.Client.IntegrationTests.Tenor
{
    using EnergyTrading.MDM.Client.Services;

    using MDM.Client.IntegrationTests.TenorType;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RWEST.Nexus.MDM.Contracts;

    public static class TenorData
    {
        public static Tenor PostBasicEntity()
        {
            var mdmService = ServiceLocator.Current.GetInstance<IMdmEntityService<Tenor>>();

            var entity = ObjectMother.Create<Tenor>();
            var mappings = entity.Identifiers;
            entity.Identifiers = new NexusIdList();
            SetAdditionalData(entity);
            var response = mdmService.Create(entity);

            Assert.IsTrue(response.IsValid, "###Error : " + response.Code + " : " + (response.Fault == null ? string.Empty : response.Fault.Message + " : " + response.Fault.Reason));

            var createdEntity = response.Message;

            foreach (var identifier in mappings)
            {
                var mappingResponse = mdmService.CreateMapping(createdEntity.ToMdmKey(), identifier);
                Assert.IsTrue(mappingResponse.IsValid);
                createdEntity.Identifiers.Add(identifier);
            }

            return createdEntity;
        }

        private static void SetAdditionalData(Tenor entity)
        {
            entity.Details.Delivery = new DateRange();
            entity.Details.Traded = new DateRange();
            entity.Details.TenorType = TenorTypeData.PostBasicEntity().ToMdmId().ToEntityId();
        }
    }
}
