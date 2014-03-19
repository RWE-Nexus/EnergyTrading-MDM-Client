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
    public class when_the_MdmFaulHandler_has_a_different_status_code_to_the_one_expected : SpecBase<MdmFaultHandler>
    {
        private MdmFaultException exception;
        private Fault fault;

        protected override MdmFaultHandler Establish_context()
        {
            this.fault = new Fault { Message = "Fault" };

            return new MdmFaultHandler();
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
            catch (MdmFaultException e)
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
            Assert.IsInstanceOfType(this.exception, typeof(MdmFaultException));
        }
    }
}