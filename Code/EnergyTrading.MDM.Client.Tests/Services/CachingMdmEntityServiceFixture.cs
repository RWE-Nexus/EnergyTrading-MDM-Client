namespace EnergyTrading.Mdm.Client.Tests.Services
{
    using EnergyTrading.Mdm.Client.Services;
    using EnergyTrading.Mdm.Contracts;
    using EnergyTrading.Test;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class when_a_request_is_made_to_the_caching_mdm_entity_service_to_delete_a_mapping : SpecBaseAutoMocking<CachingMdmEntityService<SourceSystem>>
    {
        protected override void Because_of()
        {
            this.Sut.DeleteMapping(1, 33);
        } 

       [TestMethod] 
       public void should_call_the_mdm_entity_service_to_delete_the_mapping()
       {
           this.Mock<IMdmEntityService<SourceSystem>>().Verify(service => service.DeleteMapping(1, 33));
       }
    }
}