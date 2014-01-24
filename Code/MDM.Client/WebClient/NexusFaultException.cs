namespace EnergyTrading.MDM.Client.WebClient
{
    using System;

    using RWEST.Nexus.MDM.Contracts;

    public class NexusFaultException : Exception
    {
        public NexusFaultException(Fault fault)
        {
            this.Fault = fault;
        }

        public Fault Fault { get; private set; }
    }
}