namespace EnergyTrading.Mdm.Client.Tests.WebClient
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using EnergyTrading.Mdm.Client.WebApi.WebApiClient;
    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;
    using EnergyTrading.Test;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class using_a_StandardFaultHandler_when_a_response_has_a_different_status_code_to_the_one_expected : SpecBase<StandardFaultHandler>
    {
        private bool result;

        protected override StandardFaultHandler Establish_context()
        {
            return new StandardFaultHandler();
        }

        protected override void Because_of()
        {
            var fault = new Fault();
            var response = new HttpResponseMessage
            {
                Content = new ObjectContent<Fault>(fault, new XmlMediaTypeFormatter()),
                StatusCode = HttpStatusCode.InternalServerError
            };

            try
            {
                this.result = this.Sut.Handle(response, HttpStatusCode.OK);
            }
            catch (MdmFaultException)
            {
            }
        }

        [TestMethod]
        public void should_return_false()
        {
            Assert.AreEqual(false, this.result);
        }
    }
}
