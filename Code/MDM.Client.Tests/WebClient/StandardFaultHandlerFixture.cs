namespace EnergyTrading.MDM.Client.Tests.WebClient
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using EnergyTrading.MDM.Client.WebClient;
    using EnergyTrading.Test;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RWEST.Nexus.MDM.Contracts;

    using StandardFaultHandler = EnergyTrading.MDM.Client.WebApiClient.StandardFaultHandler;

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
            catch (NexusFaultException)
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
