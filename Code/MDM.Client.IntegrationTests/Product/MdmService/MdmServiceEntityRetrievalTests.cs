﻿// <autogenerated>
//   This file was generated by T4 code generator CreateIntegrationTestsScript.tt.
//   Any changes made to this file manually will be lost next time the file is regenerated.
// </autogenerated>

namespace MDM.Client.IntegrationTests.Product.MdmService
{
	using System.Configuration;
    using System.Linq;
    using System.Net;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class MdmServiceBasicIntegrationTests : MdmServiceIntegrationTestBase
    {
        private Product product;

        protected override void OnSetup()
        {
			ConfigurationManager.AppSettings["MdmCaching"] = false.ToString();

            base.OnSetup();

            product = ProductData.PostBasicEntity();
        }

        [TestMethod]
        public void ShouldGetByIntId()
        {
            // given
            var id = int.Parse(product.ToNexusId().Identifier);

            // when
            var response = MdmService.Get<Product>(id);

            // then
                        Assert.IsTrue(response.IsValid, "###Error : " + response.Code + " : " + (response.Fault == null ? string.Empty : response.Fault.Message + " : " + response.Fault.Reason));;
            Check(product, response.Message);
        }

        [TestMethod]
        public void ShouldGetByNexusId()
        {
            // given
            var nexusId = product.ToNexusId();

            // when
            var response = MdmService.Get<Product>(nexusId);

            // then
                        Assert.IsTrue(response.IsValid, "###Error : " + response.Code + " : " + (response.Fault == null ? string.Empty : response.Fault.Message + " : " + response.Fault.Reason));;
            Check(product, response.Message);
        }

        [TestMethod]
        public void ShouldGetMapping()
        {
            // given
            var id = int.Parse(product.ToNexusId().Identifier);

            // when
            var response = MdmService.GetMapping<Product>(id, nexusId => nexusId.SystemName == "Endur");

            // then
                        Assert.IsTrue(response.IsValid, "###Error : " + response.Code + " : " + (response.Fault == null ? string.Empty : response.Fault.Message + " : " + response.Fault.Reason));;
            Assert.AreEqual(product.Identifiers.First(i => i.SystemName == "Endur").Identifier, response.Message.Identifier);
        }

        [TestMethod]
        public void ShouldMap()
        {
            // given
            var id = int.Parse(product.ToNexusId().Identifier);

            // when
            var response = MdmService.Map<Product>(id, "Endur");

            // then
                        Assert.IsTrue(response.IsValid, "###Error : " + response.Code + " : " + (response.Fault == null ? string.Empty : response.Fault.Message + " : " + response.Fault.Reason));;
            Assert.AreEqual(product.Identifiers.First(i => i.SystemName == "Endur").Identifier, response.Message.Identifier);
        }

        [TestMethod]
        public void ShouldCrossMap()
        {
            // given
            var sourceIdentifier = product.Identifiers.First(i => i.SystemName == "Endur");

            // when
            var response = MdmService.CrossMap<Product>(sourceIdentifier, "Trayport");

            // then
                        Assert.IsTrue(response.IsValid, "###Error : " + response.Code + " : " + (response.Fault == null ? string.Empty : response.Fault.Message + " : " + response.Fault.Reason));;
            Assert.AreEqual(product.Identifiers.First(i => i.SystemName == "Trayport").Identifier, response.Message.Mappings[0].Identifier);
        }
    }
}