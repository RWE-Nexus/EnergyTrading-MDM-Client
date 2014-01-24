namespace EnergyTrading.MDM.Client.Model
{
    using System.Collections.Generic;

    using RWEST.Nexus.MDM.Contracts;

    public class TenorType : IMdmModelEntity<RWEST.Nexus.MDM.Contracts.TenorType>
    {
        public TenorType()
        {
            this.Tenors = new List<Tenor>();
        }

        public RWEST.Nexus.MDM.Contracts.TenorType Source { get; set; }

        public List<Tenor> Tenors { get; set; }
    }
}