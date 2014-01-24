namespace EnergyTrading.MDM.Client.Tests.Services.Model
{
    using System;

    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.Services.Model;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using RWEST.Nexus.MDM.Contracts;

    [TestClass]
    public class ModelProductCurveServiceFixture
    {
        private Mock<IMdmModelEntityService> mdmService;
        private ModelProductCurveService service;

        [TestInitialize]
        public void SetUp()
        {
            this.mdmService = new Mock<IMdmModelEntityService>();
            this.service = new ModelProductCurveService(this.mdmService.Object);
        }

        [TestMethod]
        public void Return_null_if_contract_is_null()
        {
            var productCurve = this.service.Get(null);

            Assert.IsNull(productCurve);
        }

        [TestMethod]
        public void Source_is_contract()
        {
            var contract = new ProductCurve();

            var productCurve = this.service.Get(contract);

            Assert.AreEqual(contract, productCurve.Source);
        }

        [TestMethod]
        public void Name_comes_from_contract_details()
        {
            const string Name = "name";
            var contract = CreateProductCurve(details => details.Name = Name);

            var productCurve = this.service.Get(contract);

            Assert.AreEqual(Name, productCurve.Name);
        }

        [TestMethod]
        public void Curve_comes_from_service()
        {
            const int CurveId = 123;
            var contract = CreateProductCurve(details => details.Curve = CurveId.ToEntityId());
            var curve = this.mdmService.StubModel<Curve, EnergyTrading.MDM.Client.Model.Curve>(CurveId);

            var productCurve = this.service.Get(contract);

            Assert.AreEqual(curve, productCurve.Curve);
        }

        [TestMethod]
        public void Product_curve_type_comes_from_contract_details()
        {
            const string ProductCurveType = "productCurveType";
            var contract = CreateProductCurve(details => details.ProductCurveType = ProductCurveType);

            var productCurve = this.service.Get(contract);

            Assert.AreEqual(ProductCurveType, productCurve.ProductCurveType);
        }

        [TestMethod]
        public void Projection_method_comes_from_contract_details()
        {
            const string ProjectionMethod = "projectionMethod";
            var contract = CreateProductCurve(details => details.ProjectionMethod = ProjectionMethod);

            var productCurve = this.service.Get(contract);

            Assert.AreEqual(ProjectionMethod, productCurve.ProjectionMethod);
        }

        private static ProductCurve CreateProductCurve(Action<ProductCurveDetails> configure)
        {
            var contract = new ProductCurve();
            configure(contract.Details);
            return contract;
        }
    }
}