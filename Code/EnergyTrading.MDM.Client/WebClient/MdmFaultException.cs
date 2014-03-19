namespace EnergyTrading.Mdm.Client.WebClient
{
    using System;

    using EnergyTrading.Mdm.Contracts;

    public class MdmFaultException : Exception
    {
        public MdmFaultException(Fault fault)
        {
            this.Fault = fault;
        }

        public Fault Fault { get; private set; }
    }
}