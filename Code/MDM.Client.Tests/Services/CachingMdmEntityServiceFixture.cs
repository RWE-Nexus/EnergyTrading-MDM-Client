namespace EnergyTrading.MDM.Client.Tests.Services
{
    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.Test;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class when_a_request_is_made_to_the_caching_mdm_entity_service_to_delete_a_mapping : SpecBaseAutoMocking<CachingMdmEntityService<Person>>
    {
        protected override void Because_of()
        {
            this.Sut.DeleteMapping(1, 33);
        } 

       [TestMethod] 
       public void should_call_the_mdm_entity_service_to_delete_the_mapping()
       {
           this.Mock<IMdmEntityService<Person>>().Verify(service => service.DeleteMapping(1, 33));
       }
    }
}