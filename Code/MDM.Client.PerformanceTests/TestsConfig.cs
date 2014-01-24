namespace MDM.Client.PerformanceTests
{
    using EnergyTrading.Console;

    public class TestsConfig
    {
        private string uris = "http://s930a4014/SIT_MdmService, http://fos-t1/services/mdm_sit";
        private string systemNames = "CommercialDesktop:Gas UK, CommercialDesktop:Gas European, CommercialDesktop:Coal";
        private string entityNames = "Broker,Party";
        private int searchesPerEntity = 3;

        [CommandLineSwitch("Uris", "Comma separated list of uris against which to run tests")]
        public string Uris
        {
            get { return uris; }
            set { uris = value; }
        }

        [CommandLineSwitch("SystemNames", "Comma separated list of system names against which to run searches")]
        public string SystemNames
        {
            get { return systemNames; }
            set { systemNames = value; }
        }

        [CommandLineSwitch("EntityNames", "Entities to search for")]
        public string EntityNames
        {
            get { return entityNames; }
            set { entityNames = value; }
        }

        [CommandLineSwitch("SearchesPerEntity", "Number of searches to execute against each entity to obtain averages")]
        public int SearchesPerEntity
        {
            get { return searchesPerEntity; }
            set { searchesPerEntity = value; }
        }
    }
}

