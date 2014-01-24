namespace EnergyTrading.MDM.Client.Tests.Services
{
    using System;
    using System.Net;

    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.WebClient;
    using EnergyTrading.Test;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class MdmEntityServiceFixture : Fixture
    {
        [TestMethod]
        public void GetMissingEntityById()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<Commodity>("commodity", requester.Object);

            var response = new WebResponse<Commodity>
            {
                Code = HttpStatusCode.NotFound,
                IsValid = false,
                Message = null,
                Tag = "1"
            };

            requester.Setup(x => x.Request<Commodity>(It.IsAny<string>())).Returns(response);

            // Act
            var candidate = service.Get(1);

            // Assert
            Assert.AreSame(response, candidate);
        }

        [TestMethod]
        public void GetById()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<Commodity>("commodity", requester.Object);

            var entity = new Commodity
            {
                Details = new CommodityDetails
                {
                    Name = "Gas"
                },
                Identifiers = new NexusIdList
                {
                    new NexusId { SystemName = "Nexus", Identifier = "1", IsNexusId = true},
                    new NexusId { SystemName = "Trayport", Identifier = "A"},
                    new NexusId { SystemName = "Endur", Identifier = "B"},
                }
            };
            var expected = new WebResponse<Commodity>
            {
                Code = HttpStatusCode.OK,
                Message = entity,
                Tag = "1"
            };

            requester.Setup(x => x.Request<Commodity>(It.IsAny<string>())).Returns(expected);

            // Act
            var candidate = service.Get(1);

            // Assert
            Assert.AreSame(expected, candidate, "Entities differ");
        }

        [TestMethod]
        public void GetByIdValidAt()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<Commodity>("commodity", requester.Object);

            var entity = new Commodity
            {
                Details = new CommodityDetails
                {
                    Name = "Gas"
                },
                Identifiers = new NexusIdList
                {
                    new NexusId { SystemName = "Nexus", Identifier = "1", IsNexusId = true},
                    new NexusId { SystemName = "Trayport", Identifier = "A"},
                    new NexusId { SystemName = "Endur", Identifier = "B"},
                }
            };
            var expected = new WebResponse<Commodity>
            {
                Code = HttpStatusCode.OK,
                Message = entity,
                Tag = "1"
            };

            requester.Setup(x => x.Request<Commodity>(It.IsAny<string>())).Returns(expected);

            var now = DateTime.Now;
            string dateFormatString = "yyyy-MM-dd'T'HH:mm:ss.fffffffZ";

            // Act
            service.Get(1, now);

            // Assert
            requester.Verify(x => x.Request<Commodity>(It.Is<string>(s => s.Split('?')[1].Contains("as-of=" + now.ToString(dateFormatString)))), Times.Once());
        }

        [TestMethod]
        public void GetByMapping()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<Commodity>("commodity", requester.Object);

            var entity = new Commodity
            {
                Details = new CommodityDetails
                {
                    Name = "Gas"
                },
                Identifiers = new NexusIdList
                {
                    new NexusId { SystemName = "Nexus", Identifier = "1", IsNexusId = true},
                    new NexusId { SystemName = "Trayport", Identifier = "A"},
                    new NexusId { SystemName = "Endur", Identifier = "B"},
                }
            };
            var expected = new WebResponse<Commodity>
            {
                Code = HttpStatusCode.OK,
                Message = entity,
                Tag = "1"
            };
            requester.Setup(x => x.Request<Commodity>(It.IsAny<string>())).Returns(expected);

            var identifier = new Mapping { SystemName = "Trayport", Identifier = "A" };

            // Act
            var candidate = service.Get(identifier);

            // Assert
            Assert.AreSame(expected, candidate, "Entities differ");            
        }

        [TestMethod]
        public void GetByMappingUrlEncodesCorrectly()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<Commodity>("commodity", requester.Object);

            var entity = new Commodity
            {
                Details = new CommodityDetails
                {
                    Name = "Gas"
                },
                Identifiers = new NexusIdList
                {
                    new NexusId { SystemName = "Trayport", Identifier = "1&1"},
                }
            };
            var expected = new WebResponse<Commodity>
            {
                Code = HttpStatusCode.OK,
                Message = entity,
                Tag = "1"
            };
            requester.Setup(x => x.Request<Commodity>(It.IsAny<string>())).Returns(expected);

            var identifier = new Mapping { SystemName = "S&P", Identifier = "1&1" };

            // Act
            var candidate = service.Get(identifier);

            // Assert
            requester.Verify(x => x.Request<Commodity>("commodity/map?source-system=S%26P&mapping-string=1%261"));
        }


        [TestMethod]
        public void DeleteMapping()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<Person>("person", requester.Object);

            requester.Setup(x => x.Delete<Person>("person/77/mapping/33")).Returns(new WebResponse<Person>() { Code = HttpStatusCode.OK, IsValid = true });

            // Act
            var result = service.DeleteMapping(77, 33);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, result.Code, "Status code differ");            
            Assert.AreEqual(true, result.IsValid, "IsValid differ");            
        }

        [TestMethod]
        public void DeleteMappingFails()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<Person>("person", requester.Object);
            var fault = new Fault() { Message = "faulting"};

            requester.Setup(x => x.Delete<Person>("person/77/mapping/33")).Returns(new WebResponse<Person>() { Code = HttpStatusCode.InternalServerError, IsValid = false, Fault = fault});

            // Act
            var result = service.DeleteMapping(77, 33);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.Code, "Status code differ");            
            Assert.AreEqual(false, result.IsValid, "IsValid differ");
            Assert.AreEqual(fault, result.Fault, "Fault not returned");
        }

        [TestMethod]
        public void CreateEntity()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<Person>("person", requester.Object);
            var person = new Person();
            var response = new WebResponse<Person>() { Code = HttpStatusCode.OK, IsValid = true };

            requester.Setup(x => x.Create("person", person)).Returns(response);
            requester.Setup(x => x.Request<Person>(It.IsAny<string>()))
                .Returns(new WebResponse<Person>
                             {
                                 IsValid = true, 
                                 Code = HttpStatusCode.OK,
                                 Message = person, 
                             });

            // Act
            var result = service.Create(person);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, result.Code, "Status code differ");            
            Assert.AreEqual(true, result.IsValid, "IsValid differ");            
        }

        [TestMethod]
        public void CreateEntityFails()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<Person>("person", requester.Object);
            var fault = new Fault() { Message = "faulting"};

            requester.Setup(x => x.Create<Person>(It.IsAny<string>(), It.IsAny<Person>())).Returns(new WebResponse<Person>() { Code = HttpStatusCode.InternalServerError, IsValid = false, Fault = fault});

            // Act
            var result = service.Create(new Person());

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.Code, "Status code differ");            
            Assert.AreEqual(false, result.IsValid, "IsValid differ");
            Assert.AreEqual(fault, result.Fault, "Fault not returned");
        }

        [TestMethod]
        public void GetMapping()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<Commodity>("commodity", requester.Object);

            var tpIdentifier = new NexusId { SystemName = "Trayport", Identifier = "A" };
            var commodity = new Commodity
            {
                Details = new CommodityDetails
                {
                    Name = "Gas"
                },
                Identifiers = new NexusIdList
                {
                    new NexusId { SystemName = "Nexus", Identifier = "1", IsNexusId = true},
                    tpIdentifier,
                    new NexusId { SystemName = "Endur", Identifier = "B"},
                }
            };

            var response = new WebResponse<Commodity>
            {
                Code = HttpStatusCode.OK,
                Message = commodity,
                Tag = "1"
            };

            requester.Setup(x => x.Request<Commodity>(It.IsAny<string>())).Returns(response);

            // Act
            var candidate = service.Map(1, "Trayport");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, candidate.Code, "Code differs");
            Assert.AreEqual(true, candidate.IsValid, "IsValid differs");
            Assert.AreSame(tpIdentifier, candidate.Message, "Message differs");
        }

        [TestMethod]
        public void GetMappingMissingEntity()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<Commodity>("commodity", requester.Object);

            var response = new WebResponse<Commodity>
            {
                Code = HttpStatusCode.NotFound,
                IsValid = false,
            };

            requester.Setup(x => x.Request<Commodity>(It.IsAny<string>())).Returns(response);

            // Act
            var candidate = service.Map(1, "Trayport");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, candidate.Code, "Code differs");
            Assert.AreEqual(false, candidate.IsValid, "IsValid differs");
        }

        [TestMethod]
        public void GetMappingMissingSystemId()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<Commodity>("commodity", requester.Object);

            var tpIdentifier = new NexusId { SystemName = "Trayport", Identifier = "A" };
            var entity = new Commodity
            {
                Details = new CommodityDetails
                {
                    Name = "Gas"
                },
                Identifiers = new NexusIdList
                {
                    new NexusId { SystemName = "Nexus", Identifier = "1", IsNexusId = true},
                    tpIdentifier,
                    new NexusId { SystemName = "Endur", Identifier = "B"},
                }
            };

            var response = new WebResponse<Commodity>
            {
                Code = HttpStatusCode.OK,
                Message = entity,
                Tag = "1"
            };

            requester.Setup(x => x.Request<Commodity>(It.IsAny<string>())).Returns(response);

            // Act
            var candidate = service.Map(1, "Fred");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, candidate.Code, "Code differs");
            Assert.AreEqual(false, candidate.IsValid, "IsValid differs");
        }
    }
}