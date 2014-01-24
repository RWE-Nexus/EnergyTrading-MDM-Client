namespace EnergyTrading.MDM.Client.Tests.WebClient
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;

    using EnergyTrading.MDM.Client.WebApiClient;
    using EnergyTrading.MDM.Client.WebClient;
    using EnergyTrading.Test;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class when_a_call_is_made_to_the_message_requester_to_update_an_entity_and_it_succeeds : SpecBaseAutoMocking<MessageRequester>
    {
        private WebResponse<Person> response;

        private Person bob;

        protected override void Establish_context()
        {
            base.Establish_context();
            this.bob = new Person() { Details = new PersonDetails() { Forename = "Bob" } };

            this.Mock<IHttpClientFactory>().Setup(factory => factory.Create(It.IsAny<string>())).Returns(this.RegisterMock<IHttpClient>().Object);

            var mockResponse = new HttpResponseMessage()
                                   {
                                       StatusCode = HttpStatusCode.NoContent,
                                   };
            mockResponse.Headers.ETag = new EntityTagHeaderValue(string.Concat("\"", "etagvalue", "\""), true);
            mockResponse.Headers.Location = new Uri("/person/1", UriKind.Relative);
            this.Mock<IHttpClient>()
                .Setup(client => client.Post(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Returns(() => mockResponse);
        }

        protected override void Because_of()
        {
            this.response = this.Sut.Update("http://someuri/person/1", "etagvalue", this.bob);
        }

        [TestMethod]
        public void should_return_a_valid_response()
        {
            Assert.AreEqual(this.response.IsValid, true); 
        }

        [TestMethod]
        public void should_return_a_ok_status_code()
        {
            Assert.AreEqual(HttpStatusCode.NoContent, this.response.Code);
        }
    }

    [TestClass]
    public class when_a_call_is_made_to_the_message_requester_to_update_an_entity_and_a_fault_occurs : SpecBaseAutoMocking<MessageRequester>
    {
        private WebResponse<Person> response;
        private Person bob;

        protected override void Establish_context()
        {
            base.Establish_context();
            this.bob = new Person(); 

            this.Mock<IHttpClientFactory>().Setup(factory => factory.Create("http://someuri/")).Returns(this.RegisterMock<IHttpClient>().Object);
            var content = new ObjectContent<Fault>(new Fault() { Message = "Fault!"}, new XmlMediaTypeFormatter());
            this.Mock<IHttpClient>().Setup(client => client.Post(It.IsAny<string>(), It.IsAny<HttpContent>())).
                Returns(() => new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = content});
        }

        protected override void Because_of()
        {
            this.response = this.Sut.Update("http://someuri/", "etagvalue", this.bob);
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
    public class when_a_call_is_made_to_the_message_requester_to_update_an_entity_and_an_exception_is_thrown_by_the_service : SpecBaseAutoMocking<MessageRequester>
    {
        private WebResponse<Person> response;
        private Person bob;

        protected override void Establish_context()
        {
            base.Establish_context();
            this.bob = new Person();

            this.Mock<IHttpClientFactory>().Setup(factory => factory.Create("http://someuri/")).Returns(this.RegisterMock<IHttpClient>().Object);
            this.Mock<IHttpClient>().Setup(client => client.Post(It.IsAny<string>(), It.IsAny<HttpContent>())).Throws(new Exception("unkown exception")); 
        }

        protected override void Because_of()
        {
            this.response = this.Sut.Update<Person>("http://someuri/", "etagvalue", this.bob);
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

