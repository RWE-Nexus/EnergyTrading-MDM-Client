namespace MDM.Client.IntegrationTests.Checkers
{
    using EnergyTrading.Mdm.Contracts;
    using EnergyTrading.Test;

    public class MdmIdChecker : Checker<MdmId>
    {
        public MdmIdChecker()
        {
            this.Compare(x => x.SystemId);
            this.Compare(x => x.SystemName);
            this.Compare(x => x.Identifier);
        }
    }
}