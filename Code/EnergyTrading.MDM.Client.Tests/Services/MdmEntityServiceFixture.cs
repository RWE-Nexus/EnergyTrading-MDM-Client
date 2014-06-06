namespace EnergyTrading.Mdm.Client.Tests.Services
{
    using System;
    using System.Net;

    using EnergyTrading.Mdm.Client.Services;
    using EnergyTrading.Mdm.Client.WebClient;
    using EnergyTrading.Mdm.Contracts;
    using EnergyTrading.Test;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class MdmEntityServiceFixture : Fixture
    {
        [Test]
        public void GetMissingEntityById()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<SourceSystem>("sourcesystem", requester.Object);

            var response = new WebResponse<SourceSystem>
            {
                Code = HttpStatusCode.NotFound,
                IsValid = false,
                Message = null,
                Tag = "1"
            };

            requester.Setup(x => x.Request<SourceSystem>(It.IsAny<string>())).Returns(response);

            // Act
            var candidate = service.Get(1);

            // Assert
            Assert.AreSame(response, candidate);
        }

        [Test]
        public void GetById()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<SourceSystem>("sourcesystem", requester.Object);

            var entity = new SourceSystem
            {
                Details = new SourceSystemDetails
                {
                    Name = "Gas"
                },
                Identifiers = new MdmIdList
                {
                    new MdmId { SystemName = "Nexus", Identifier = "1", IsMdmId = true},
                    new MdmId { SystemName = "Trayport", Identifier = "A"},
                    new MdmId { SystemName = "Endur", Identifier = "B"},
                }
            };
            var expected = new WebResponse<SourceSystem>
            {
                Code = HttpStatusCode.OK,
                Message = entity,
                Tag = "1"
            };

            requester.Setup(x => x.Request<SourceSystem>(It.IsAny<string>())).Returns(expected);

            // Act
            var candidate = service.Get(1);

            // Assert
            Assert.AreSame(expected, candidate, "Entities differ");
        }

        [Test]
        public void GetByIdValidAt()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<SourceSystem>("sourcesystem", requester.Object);

            var entity = new SourceSystem
            {
                Details = new SourceSystemDetails
                {
                    Name = "Gas"
                },
                Identifiers = new MdmIdList
                {
                    new MdmId { SystemName = "Nexus", Identifier = "1", IsMdmId = true},
                    new MdmId { SystemName = "Trayport", Identifier = "A"},
                    new MdmId { SystemName = "Endur", Identifier = "B"},
                }
            };
            var expected = new WebResponse<SourceSystem>
            {
                Code = HttpStatusCode.OK,
                Message = entity,
                Tag = "1"
            };

            requester.Setup(x => x.Request<SourceSystem>(It.IsAny<string>())).Returns(expected);

            var now = DateTime.Now;
            string dateFormatString = "yyyy-MM-dd'T'HH:mm:ss.fffffffZ";

            // Act
            service.Get(1, now);

            // Assert
            requester.Verify(x => x.Request<SourceSystem>(It.Is<string>(s => s.Split('?')[1].Contains("as-of=" + now.ToString(dateFormatString)))), Times.Once());
        }

        [Test]
        public void GetByMapping()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<SourceSystem>("sourcesystem", requester.Object);

            var entity = new SourceSystem
            {
                Details = new SourceSystemDetails
                {
                    Name = "Gas"
                },
                Identifiers = new MdmIdList
                {
                    new MdmId { SystemName = "Nexus", Identifier = "1", IsMdmId = true},
                    new MdmId { SystemName = "Trayport", Identifier = "A"},
                    new MdmId { SystemName = "Endur", Identifier = "B"},
                }
            };
            var expected = new WebResponse<SourceSystem>
            {
                Code = HttpStatusCode.OK,
                Message = entity,
                Tag = "1"
            };
            requester.Setup(x => x.Request<SourceSystem>(It.IsAny<string>())).Returns(expected);

            var identifier = new Mapping { SystemName = "Trayport", Identifier = "A" };

            // Act
            var candidate = service.Get(identifier);

            // Assert
            Assert.AreSame(expected, candidate, "Entities differ");            
        }

        [Test]
        public void GetByMappingUrlEncodesCorrectly()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<SourceSystem>("sourcesystem", requester.Object);

            var entity = new SourceSystem
            {
                Details = new SourceSystemDetails
                {
                    Name = "Gas"
                },
                Identifiers = new MdmIdList
                {
                    new MdmId { SystemName = "Trayport", Identifier = "1&1"},
                }
            };
            var expected = new WebResponse<SourceSystem>
            {
                Code = HttpStatusCode.OK,
                Message = entity,
                Tag = "1"
            };
            requester.Setup(x => x.Request<SourceSystem>(It.IsAny<string>())).Returns(expected);

            var identifier = new Mapping { SystemName = "S&P", Identifier = "1&1" };

            // Act
            var candidate = service.Get(identifier);

            // Assert
            requester.Verify(x => x.Request<SourceSystem>("sourcesystem/map?source-system=S%26P&mapping-string=1%261"));
        }


        [Test]
        public void DeleteMapping()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<SourceSystem>("sourcesystem", requester.Object);

            requester.Setup(x => x.Delete<SourceSystem>("sourcesystem/77/mapping/33")).Returns(new WebResponse<SourceSystem>() { Code = HttpStatusCode.OK, IsValid = true });

            // Act
            var result = service.DeleteMapping(77, 33);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, result.Code, "Status code differ");            
            Assert.AreEqual(true, result.IsValid, "IsValid differ");            
        }

        [Test]
        public void DeleteMappingFails()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<SourceSystem>("sourcesystem", requester.Object);
            var fault = new Fault() { Message = "faulting"};

            requester.Setup(x => x.Delete<SourceSystem>("sourcesystem/77/mapping/33")).Returns(new WebResponse<SourceSystem>() { Code = HttpStatusCode.InternalServerError, IsValid = false, Fault = fault});

            // Act
            var result = service.DeleteMapping(77, 33);

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.Code, "Status code differ");            
            Assert.AreEqual(false, result.IsValid, "IsValid differ");
            Assert.AreEqual(fault, result.Fault, "Fault not returned");
        }

        [Test]
        public void CreateEntity()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<SourceSystem>("sourcesystem", requester.Object);
            var sourcesystem = new SourceSystem();
            var response = new WebResponse<SourceSystem>() { Code = HttpStatusCode.OK, IsValid = true };

            requester.Setup(x => x.Create("sourcesystem", sourcesystem)).Returns(response);
            requester.Setup(x => x.Request<SourceSystem>(It.IsAny<string>()))
                .Returns(new WebResponse<SourceSystem>
                             {
                                 IsValid = true, 
                                 Code = HttpStatusCode.OK,
                                 Message = sourcesystem, 
                             });

            // Act
            var result = service.Create(sourcesystem);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, result.Code, "Status code differ");            
            Assert.AreEqual(true, result.IsValid, "IsValid differ");            
        }

        [Test]
        public void CreateEntityFails()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<SourceSystem>("sourcesystem", requester.Object);
            var fault = new Fault() { Message = "faulting"};

            requester.Setup(x => x.Create<SourceSystem>(It.IsAny<string>(), It.IsAny<SourceSystem>())).Returns(new WebResponse<SourceSystem>() { Code = HttpStatusCode.InternalServerError, IsValid = false, Fault = fault});

            // Act
            var result = service.Create(new SourceSystem());

            // Assert
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.Code, "Status code differ");            
            Assert.AreEqual(false, result.IsValid, "IsValid differ");
            Assert.AreEqual(fault, result.Fault, "Fault not returned");
        }

        [Test]
        public void GetMapping()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<SourceSystem>("sourcesystem", requester.Object);

            var tpIdentifier = new MdmId { SystemName = "Trayport", Identifier = "A" };
            var sourcesystem = new SourceSystem
            {
                Details = new SourceSystemDetails
                {
                    Name = "Gas"
                },
                Identifiers = new MdmIdList
                {
                    new MdmId { SystemName = "Nexus", Identifier = "1", IsMdmId = true},
                    tpIdentifier,
                    new MdmId { SystemName = "Endur", Identifier = "B"},
                }
            };

            var response = new WebResponse<SourceSystem>
            {
                Code = HttpStatusCode.OK,
                Message = sourcesystem,
                Tag = "1"
            };

            requester.Setup(x => x.Request<SourceSystem>(It.IsAny<string>())).Returns(response);

            // Act
            var candidate = service.Map(1, "Trayport");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, candidate.Code, "Code differs");
            Assert.AreEqual(true, candidate.IsValid, "IsValid differs");
            Assert.AreSame(tpIdentifier, candidate.Message, "Message differs");
        }

        [Test]
        public void GetMappingMissingEntity()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<SourceSystem>("sourcesystem", requester.Object);

            var response = new WebResponse<SourceSystem>
            {
                Code = HttpStatusCode.NotFound,
                IsValid = false,
            };

            requester.Setup(x => x.Request<SourceSystem>(It.IsAny<string>())).Returns(response);

            // Act
            var candidate = service.Map(1, "Trayport");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, candidate.Code, "Code differs");
            Assert.AreEqual(false, candidate.IsValid, "IsValid differs");
        }

        [Test]
        public void GetMappingMissingSystemId()
        {
            var requester = new Mock<IMessageRequester>();
            var service = new MdmEntityService<SourceSystem>("sourcesystem", requester.Object);

            var tpIdentifier = new MdmId { SystemName = "Trayport", Identifier = "A" };
            var entity = new SourceSystem
            {
                Details = new SourceSystemDetails
                {
                    Name = "Gas"
                },
                Identifiers = new MdmIdList
                {
                    new MdmId { SystemName = "Nexus", Identifier = "1", IsMdmId = true},
                    tpIdentifier,
                    new MdmId { SystemName = "Endur", Identifier = "B"},
                }
            };

            var response = new WebResponse<SourceSystem>
            {
                Code = HttpStatusCode.OK,
                Message = entity,
                Tag = "1"
            };

            requester.Setup(x => x.Request<SourceSystem>(It.IsAny<string>())).Returns(response);

            // Act
            var candidate = service.Map(1, "Fred");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, candidate.Code, "Code differs");
            Assert.AreEqual(false, candidate.IsValid, "IsValid differs");
        }
    }
}