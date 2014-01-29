﻿// <autogenerated>
//   This file was generated by T4 code generator CreateIntegrationTestsScript.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

namespace MDM.Client.IntegrationTests.PartyOverride
{
    using EnergyTrading.MDM.Client.Services;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RWEST.Nexus.MDM.Contracts;

    public class PartyOverrideData
    {
        public static PartyOverride PostBasicEntity()
        {
            var mdmService = ServiceLocator.Current.GetInstance<IMdmEntityService<PartyOverride>>();

            var entity = ObjectMother.Create<PartyOverride>();
            var mappings = entity.Identifiers;
            entity.Identifiers = new NexusIdList();
            SetAdditionalData(entity);
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

        protected static void SetAdditionalData(PartyOverride entity)
        {
			
            entity.Details.CommodityInstrumentType = IntegrationTests.CommodityInstrumentType.CommodityInstrumentTypeData.PostBasicEntity().ToNexusId().ToEntityId();
            entity.Details.Party = IntegrationTests.Party.PartyData.PostBasicEntity().ToNexusId().ToEntityId();
            entity.Details.Broker = IntegrationTests.Broker.BrokerData.PostBasicEntity().ToNexusId().ToEntityId();
            entity.Details.MappingValue = "map";
	
        }
    }
}