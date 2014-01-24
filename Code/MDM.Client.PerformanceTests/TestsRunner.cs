namespace MDM.Client.PerformanceTests
{
    using MDM.Client.PerformanceTests.Extensions;
    using EnergyTrading.Test.TeamCity;

    public class TestsRunner
    {
        private readonly ITeamCityBuildLogger logger = new TeamCityBuildLogger();

        public void RunTests(string uris, string systemNames, string entityNames, int searchesPerEntity)
        {
            foreach (var uri in uris.SplitToList())
            {
                logger.LogSuiteStarted(uri);

                foreach (var systemName in systemNames.SplitToList())
                {
                    logger.LogSuiteStarted(systemName);
                    
                    var runner = new TestRunner(uri, searchesPerEntity);
                    foreach (var entityName in entityNames.SplitToList())
                    {
                        logger.LogTestStarted(entityName);

                        var testResults = runner.RunSearchTest(systemName, entityName);
                        LogTestResults(entityName, testResults);

                        logger.LogTestFinished(entityName);
                    }

                    logger.LogSuiteFinished(systemName);
                }
                logger.LogSuiteFinished(uri);
            }
        }

        private void LogTestResults(string entityName, TestResults results)
        {
            logger.LogTestOutput(entityName, "Entity,Number of Searches,Avg,Min,Max,Total");
            var message = string.Format("{0},{1},{2},{3},{4},{5}", entityName, results.NumberOfIterations, results.Average, results.Min, results.Max, results.Sum);
            logger.LogTestOutput(entityName, message);
        }
    }
}