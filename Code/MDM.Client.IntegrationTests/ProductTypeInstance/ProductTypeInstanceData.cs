﻿// <autogenerated>
//   This file was generated by T4 code generator CreateIntegrationTestsScript.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

namespace MDM.Client.IntegrationTests.ProductTypeInstance
{
    using EnergyTrading.MDM.Client.Services;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RWEST.Nexus.MDM.Contracts;

    public class ProductTypeInstanceData
    {
        public static ProductTypeInstance PostBasicEntity()
        {
            var mdmService = ServiceLocator.Current.GetInstance<IMdmEntityService<ProductTypeInstance>>();

            var entity = ObjectMother.Create<ProductTypeInstance>();
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

        protected static void SetAdditionalData(ProductTypeInstance entity)
        {
			
            entity.Details.ProductType = IntegrationTests.ProductType.ProductTypeData.PostBasicEntity().ToNexusId().ToEntityId();
            entity.Details.Delivery = new DateRange { StartDate = new System.DateTime(2010, 01, 01), EndDate = new System.DateTime(2020, 01, 01) };
	
        }
    }
}