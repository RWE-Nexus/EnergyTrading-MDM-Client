namespace MDM.Client.PerformanceTests
{
    using System;
    using System.Configuration;
    using System.Reflection;

    using EnergyTrading.MDM.Client.Extensions;
    using EnergyTrading.MDM.Client.Registrars;
    using EnergyTrading.MDM.Client.Services;

    using Microsoft.Practices.Unity;

    using EnergyTrading.Container.Unity;

    /// <summary>
    /// TestRunner that creates a new container context
    /// </summary>
    public class TestRunner
    {
        private readonly string uri;
        private readonly int iterationsPerEntity;
        private readonly IUnityContainer container;

        public TestRunner(string uri, int iterationsPerEntity)
        {
            this.uri = uri;
            this.iterationsPerEntity = iterationsPerEntity;
            ConfigurationManager.AppSettings["MdmCaching"] = false.ToString();
            ConfigurationManager.AppSettings["MdmUri"] = this.uri;
            this.container = CreateContainer();
        }

        public TestResults RunSearchTest(string systemName, string typeName)
        {
            var searchMethod = SearchMethod(typeName);
            var mdmService = container.Resolve<IMdmService>();
            var testResults = new TestResults(iterationsPerEntity);
            testResults.InvokeAndCollect(() => searchMethod.Invoke(mdmService, new object[] { systemName.MappingSearch() }));
            return testResults;
        }

        private static MethodInfo SearchMethod(string typeName)
        {
            var methodInfo = typeof (IMdmService).GetMethod("Search");
            var fullTypeName = string.Format("RWEST.Nexus.MDM.Contracts.{0},MDM.Contracts", typeName);
            return methodInfo.MakeGenericMethod(new[] {Type.GetType(fullTypeName)});
        }

        private IUnityContainer CreateContainer()
        {
            var unityContainer = new UnityContainer();
            unityContainer.StandardConfiguration();
            
            new MdmClientRegistrarWebApi().Register(unityContainer);
            
            return unityContainer;
        }
    }
}
