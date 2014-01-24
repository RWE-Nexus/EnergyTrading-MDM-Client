namespace MDM.Client.IntegrationTests.TenorType
{
    using EnergyTrading.MDM.Client.Services;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RWEST.Nexus.MDM.Contracts;

    public class TenorTypeData
    {
        public static TenorType PostBasicEntity()
        {
            var mdmService = ServiceLocator.Current.GetInstance<IMdmEntityService<TenorType>>();

            var entity = ObjectMother.Create<TenorType>();
            var mappings = entity.Identifiers;
            entity.Identifiers = new NexusIdList();
            var response = mdmService.Create(entity);
            
            Assert.IsTrue(response.IsValid, "###Error : " + response.Code + " : " + (response.Fault == null ? string.Empty : response.Fault.Message + " : " + response.Fault.Reason));

            var createdEntity = response.Message;

            foreach (var identifier in mappings)
            {
                var mappingResponse = mdmService.CreateMapping(createdEntity.ToNexusKey(), identifier);
                Assert.IsTrue(mappingResponse.IsValid);
                createdEntity.Identifiers.Add(identifier);
            }

            return createdEntity;
        }
    }
}
