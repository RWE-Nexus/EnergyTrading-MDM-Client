namespace MDM.Client.PerformanceTests
{
    using System;
    using EnergyTrading.Console;

    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new TestsConfig();
            var parser = new CommandLineParser(Environment.CommandLine, config);
            parser.Parse();

            new TestsRunner().RunTests(config.Uris, config.SystemNames, config.EntityNames, config.SearchesPerEntity);
        }
    }
}

