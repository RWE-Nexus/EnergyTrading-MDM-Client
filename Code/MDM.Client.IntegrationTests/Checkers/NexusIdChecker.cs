namespace MDM.Client.IntegrationTests.Checkers
{
    using RWEST.Nexus.MDM.Contracts;
    using EnergyTrading.Test;

    public class NexusIdChecker : Checker<NexusId>
    {
        public NexusIdChecker()
        {
            this.Compare(x => x.SystemId);
            this.Compare(x => x.SystemName);
            this.Compare(x => x.Identifier);
        }
    }
}