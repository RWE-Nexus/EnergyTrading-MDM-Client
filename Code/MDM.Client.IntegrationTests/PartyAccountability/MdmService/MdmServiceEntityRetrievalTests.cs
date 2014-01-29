﻿// <autogenerated>
//   This file was generated by T4 code generator CreateIntegrationTestsScript.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

namespace MDM.Client.IntegrationTests.PartyAccountability.MdmService
{
	using System.Configuration;
    using System.Linq;
    using System.Net;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class MdmServiceBasicIntegrationTests : MdmServiceIntegrationTestBase
    {
        private PartyAccountability partyaccountability;

        protected override void OnSetup()
        {
			ConfigurationManager.AppSettings["MdmCaching"] = false.ToString();

            base.OnSetup();

            partyaccountability = PartyAccountabilityData.PostBasicEntity();
        }

        [TestMethod]
        public void ShouldGetByIntId()
        {
            // given
            var id = int.Parse(partyaccountability.ToNexusId().Identifier);

            // when
            var response = MdmService.Get<PartyAccountability>(id);

            // then
            Assert.IsTrue(response.IsValid, "###Error : " + response.Code + " : " + (response.Fault == null ? string.Empty : response.Fault.Message + " : " + response.Fault.Reason));
            Check(partyaccountability, response.Message);
        }

        [TestMethod]
        public void ShouldGetByNexusId()
        {
            // given
            var nexusId = partyaccountability.ToNexusId();

            // when
            var response = MdmService.Get<PartyAccountability>(nexusId);

            // then
            Assert.IsTrue(response.IsValid, "###Error : " + response.Code + " : " + (response.Fault == null ? string.Empty : response.Fault.Message + " : " + response.Fault.Reason));
            Check(partyaccountability, response.Message);
        }

        [TestMethod]
        public void ShouldGetMapping()
        {
            // given
            var id = int.Parse(partyaccountability.ToNexusId().Identifier);

            // when
            var response = MdmService.GetMapping<PartyAccountability>(id, nexusId => nexusId.SystemName == "Endur");

            // then
            Assert.IsTrue(response.IsValid, "###Error : " + response.Code + " : " + (response.Fault == null ? string.Empty : response.Fault.Message + " : " + response.Fault.Reason));
            Assert.AreEqual(partyaccountability.Identifiers.First(i => i.SystemName == "Endur").Identifier, response.Message.Identifier);
        }

        [TestMethod]
        public void ShouldMap()
        {
            // given
            var id = int.Parse(partyaccountability.ToNexusId().Identifier);

            // when
            var response = MdmService.Map<PartyAccountability>(id, "Endur");

            // then
            Assert.IsTrue(response.IsValid, "###Error : " + response.Code + " : " + (response.Fault == null ? string.Empty : response.Fault.Message + " : " + response.Fault.Reason));
            Assert.AreEqual(partyaccountability.Identifiers.First(i => i.SystemName == "Endur").Identifier, response.Message.Identifier);
        }

        [TestMethod]
        public void ShouldCrossMap()
        {
            // given
            var sourceIdentifier = partyaccountability.Identifiers.First(i => i.SystemName == "Endur");

            // when
            var response = MdmService.CrossMap<PartyAccountability>(sourceIdentifier, "Trayport");

            // then
            Assert.IsTrue(response.IsValid, "###Error : " + response.Code + " : " + (response.Fault == null ? string.Empty : response.Fault.Message + " : " + response.Fault.Reason));
            Assert.AreEqual(partyaccountability.Identifiers.First(i => i.SystemName == "Trayport").Identifier, response.Message.Mappings[0].Identifier);
        }
    }
}