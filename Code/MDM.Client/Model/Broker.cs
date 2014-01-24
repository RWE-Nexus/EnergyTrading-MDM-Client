namespace EnergyTrading.MDM.Client.Model
{
    public class Broker : IMdmModelEntity<RWEST.Nexus.MDM.Contracts.Broker>
    {
        public RWEST.Nexus.MDM.Contracts.Broker Source { get; set; }

        public string Name { get; set; }
    }
}
