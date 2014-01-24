namespace EnergyTrading.MDM.Client.Tests.Services.Model
{
    using System.Collections.Generic;
    using System.Globalization;

    using EnergyTrading.MDM.Client.Model;
    using EnergyTrading.MDM.Client.Services;
    using EnergyTrading.MDM.Client.WebClient;

    using Moq;

    using RWEST.Nexus.MDM.Contracts;

    public static class TestHelperExtensions
    {
        public static EntityId ToEntityId(this int id)
        {
            return new EntityId { Identifier = new NexusId { Identifier = id.ToString(CultureInfo.InvariantCulture) } };
        }

        public static void StubSearch<T>(this Mock<IMdmModelEntityService> mdmService, T item = default(T)) 
            where T : class, IMdmEntity
        {
            var response = new WebResponse<IList<T>>();
            if (item != null)
            {
                response.Message = new[] { item };
            }
            
            mdmService.Setup(s => s.Search<T>(It.IsAny<EnergyTrading.Contracts.Search.Search>())).Returns(response);
        }

        public static void StubSearch<T>(this Mock<IMdmModelEntityService> mdmService, IList<T> contracts)
            where T : class, IMdmEntity, new()
        {
            var response = new WebResponse<IList<T>>();
            if (contracts != null)
            {
                response.Message = contracts;
            }

            mdmService.Setup(s => s.Search<T>(It.IsAny<EnergyTrading.Contracts.Search.Search>())).Returns(response);
        }

        public static TContract Stub<TContract>(this Mock<IMdmModelEntityService> mdmService, int id)
            where TContract : class, IMdmEntity, new()
        {
            var contract = new TContract();
            mdmService.Setup(s => s.Get<TContract>(id)).Returns(contract);
            return contract;
        }

        public static TModel StubModel<TContract, TModel>(this Mock<IMdmModelEntityService> mdmService, int id)
            where TContract : class, IMdmEntity, new()
            where TModel : IMdmModelEntity<TContract>, new()
        {
            var contract = Stub<TContract>(mdmService, id);
            var model = new TModel();
            mdmService.Setup(s => s.Get<TContract, TModel>(contract)).Returns(model);
            return model;
        }

        public static TModel StubModel<TContract, TModel>(this Mock<IMdmModelEntityService> mdmService, TContract contract)
            where TContract : class, IMdmEntity, new()
            where TModel : IMdmModelEntity<TContract>, new()
        {
            var model = new TModel();
            mdmService.Setup(s => s.Get<TContract, TModel>(contract)).Returns(model);
            return model;
        }
    }
}