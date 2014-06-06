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

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class when_a_call_is_made_to_the_message_requester_to_create_an_entity_and_it_succeeds : SpecBaseAutoMocking<MessageRequester>
    {
        private WebResponse<SourceSystem> response;

        protected override void Establish_context()
        {
            base.Establish_context();

            this.Mock<IHttpClientFactory>().Setup(factory => factory.Create("http://someuri/person")).Returns(this.RegisterMock<IHttpClient>().Object);

            // TODO: change this when we change location on mdm side
            var mockResponse = new HttpResponseMessage { StatusCode = HttpStatusCode.Created };
            mockResponse.Headers.Location = new Uri("person/1", UriKind.Relative);
            this.Mock<IHttpClient>()
                .Setup(client => client.Post(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Returns(() => mockResponse);
        }

        protected override void Because_of()
        {
            this.response = this.Sut.Create("http://someuri/person", new SourceSystem());
        }

        [Test]
        public void should_return_a_valid_response()
        {
            Assert.AreEqual(this.response.IsValid, true); 
        }

        [Test]
        public void should_return_a_location()
        {
            Assert.AreEqual("http://someuri/person/1", this.response.Location);
        }

        [Test]
        public void should_return_a_created_status_code()
        {
            Assert.AreEqual(HttpStatusCode.Created, this.response.Code);
        }
    }

    [TestFixture]
    public class when_a_call_is_made_to_the_message_requester_to_create_an_entity_and_a_fault_occurs : SpecBaseAutoMocking<MessageRequester>
    {
        private WebResponse<SourceSystem> response;

        protected override void Establish_context()
        {
            base.Establish_context();

            this.Mock<IHttpClientFactory>().Setup(factory => factory.Create("http://someuri/")).Returns(this.RegisterMock<IHttpClient>().Object);
            var fault = new Fault() { Message = "Fault!" };
            var content = new ObjectContent<Fault>(fault, new XmlMediaTypeFormatter());
            this.Mock<IHttpClient>().Setup(client => client.Post(It.IsAny<string>(), It.IsAny<HttpContent>())).Returns(() => new HttpResponseMessage() { StatusCode = HttpStatusCode.InternalServerError, Content = content});
        }

        protected override void Because_of()
        {
            this.response = this.Sut.Create("http://someuri/", new SourceSystem());
        }

        [Test]
        public void should_return_the_fault_in_the_response()
        {
            Assert.AreEqual("Fault!", this.response.Fault.Message); 
        }

        [Test]
        public void should_not_return_a_location()
        {
            Assert.AreEqual(null, this.response.Location);
        }
    }

    [TestFixture]
    public class when_a_call_is_made_to_the_message_requester_to_create_an_entity_and_an_exception_is_thrown_by_the_service : SpecBaseAutoMocking<MessageRequester>
    {
        private WebResponse<SourceSystem> response;

        protected override void Establish_context()
        {
            base.Establish_context();

            this.Mock<IHttpClientFactory>().Setup(factory => factory.Create("http://someuri/")).Returns(this.RegisterMock<IHttpClient>().Object);
            this.Mock<IHttpClient>().Setup(client => client.Post(It.IsAny<string>(), It.IsAny<HttpContent>())).Throws(new Exception("unkown exception")); 
        }

        protected override void Because_of()
        {
            this.response = this.Sut.Create("http://someuri/", new SourceSystem());
        }

        [Test]
        public void should_return_a_fault_in_the_response()
        {
            Assert.IsNotNull(this.response.Fault.Message); 
        }

        [Test]
        public void should_return_an_internal_server_error_code()
        {
            Assert.AreEqual(HttpStatusCode.InternalServerError, this.response.Code);
        }

        [Test]
        public void should_mark_the_message_as_not_valid()
        {
            Assert.AreEqual(false, this.response.IsValid);
        }

        [Test]
        public void should_return_the_excpetion_message_from_the_service()
        {
            Assert.IsTrue(this.response.Fault.Message.Contains("unkown exception"));
        }
    }
}