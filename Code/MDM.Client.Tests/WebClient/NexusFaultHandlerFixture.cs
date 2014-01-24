namespace EnergyTrading.MDM.Client.Tests.WebClient
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using EnergyTrading.MDM.Client.WebClient;
    using EnergyTrading.Test;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RWEST.Nexus.MDM.Contracts;

    using NexusFaultHandler = EnergyTrading.MDM.Client.WebApiClient.NexusFaultHandler;

    [TestClass]
    public class when_the_NexusFaulHandler_has_a_different_status_code_to_the_one_expected : SpecBase<NexusFaultHandler>
    {
        private NexusFaultException exception;
        private Fault fault;

        protected override NexusFaultHandler Establish_context()
        {
            this.fault = new Fault { Message = "Fault" };

            return new NexusFaultHandler();
        }

        protected override void Because_of()
        {
            try
            {
                this.Sut.Handle(
                    new HttpResponseMessage
                        {
                            Content = new ObjectContent<Fault>(this.fault, new XmlMediaTypeFormatter()), 
                            StatusCode = HttpStatusCode.InternalServerError
                        }, 
                    HttpStatusCode.OK);
            }
            catch (NexusFaultException e)
            {
                this.exception = e;
            }
        }

        [TestMethod]
        public void should_return_the_fault_data()
        {
            Assert.AreEqual(this.fault.Message, this.exception.Fault.Message);
        }

        [TestMethod]
        public void should_throw_a_fault_exception()
        {
            Assert.IsInstanceOfType(this.exception, typeof(NexusFaultException));
        }
    }
}