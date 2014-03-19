namespace EnergyTrading.Mdm.Client.Tests.WebClient
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using EnergyTrading.Mdm.Client.WebApi.WebApiClient;
    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;
    using EnergyTrading.Test;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    [TestClass]
    public class when_a_call_is_made_to_the_message_requester_to_delete_an_mapping_and_it_succeeds : SpecBaseAutoMocking<MessageRequester>
    {
        private WebResponse<SourceSystem> response;

        protected override void Establish_context()
        {
            base.Establish_context();

            this.Mock<IHttpClientFactory>().Setup(factory => factory.Create(It.IsAny<string>())).Returns(this.RegisterMock<IHttpClient>().Object);

            this.Mock<IHttpClient>().Setup(client => client.Delete(It.IsAny<string>())).
                Returns(() => new HttpResponseMessage() { StatusCode = HttpStatusCode.OK });
        }

        protected override void Because_of()
        {
            this.response = this.Sut.Delete<SourceSystem>("http://someuri/person/1/mapping/23");
        }

        [TestMethod]
        public void should_return_a_valid_response()
        {
            Assert.AreEqual(true, this.response.IsValid); 
        }

        [TestMethod]
        public void should_return_a_ok_status_code()
        {
            Assert.AreEqual(HttpStatusCode.OK, this.response.Code);
        }
    }

    [TestClass]
    public class when_a_call_is_made_to_the_message_requester_to_delete_an_entity_and_a_fault_occurs : SpecBaseAutoMocking<MessageRequester>
    {
        private WebResponse<SourceSystem> response;

        protected override void Establish_context()
        {
            base.Establish_context();

            this.Mock<IHttpClientFactory>().Setup(factory => factory.Create(It.IsAny<string>())).Returns(this.RegisterMock<IHttpClient>().Object);
            var content = new ObjectContent<Fault>(new Fault() { Message = "Fault!"}, new XmlMediaTypeFormatter());
            this.Mock<IHttpClient>().Setup(client => client.Delete(It.IsAny<string>())).Returns(() => new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = content});
        }

        protected override void Because_of()
        {
            this.response = this.Sut.Delete<SourceSystem>("http://someuri/");
        }

        [TestMethod]
        public void should_return_the_fault_in_the_response()
        {
            Assert.AreEqual("Fault!", this.response.Fault.Message); 
        }

        [TestMethod]
        public void should_not_return_a_location()
        {
            Assert.AreEqual(null, this.response.Location);
        }
    }

    [TestClass]
    public class when_a_call_is_made_to_the_message_requester_to_delete_an_entity_and_an_exception_is_thrown_by_the_service : SpecBaseAutoMocking<MessageRequester>
    {
        private WebResponse<SourceSystem> response;

        protected override void Establish_context()
        {
            base.Establish_context();

            this.Mock<IHttpClientFactory>().Setup(factory => factory.Create(It.IsAny<string>())).Returns(this.RegisterMock<IHttpClient>().Object);
            this.Mock<IHttpClient>().Setup(client => client.Delete(It.IsAny<string>())).Throws(new Exception("unkown exception")); 
        }

        protected override void Because_of()
        {
            this.response = this.Sut.Delete<SourceSystem>("http://someuri/person/1/mapping/123");
        }

        [TestMethod]
        public void should_return_a_fault_in_the_response()
        {
            Assert.IsNotNull(this.response.Fault.Message); 
        }

        [TestMethod]
        public void should_return_an_internal_server_error_code()
        {
            Assert.AreEqual(HttpStatusCode.InternalServerError, this.response.Code);
        }

        [TestMethod]
        public void should_mark_the_message_as_not_valid()
        {
            Assert.AreEqual(false, this.response.IsValid);
        }

        [TestMethod]
        public void should_return_the_excpetion_message_from_the_service()
        {
            Assert.IsTrue(this.response.Fault.Message.Contains("unkown exception"));
        }
    }
}