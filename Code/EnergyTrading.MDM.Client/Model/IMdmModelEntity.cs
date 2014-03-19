namespace EnergyTrading.Mdm.Client.Model
{
    using EnergyTrading.Mdm.Contracts;

    /// <summary>
    /// Model wrapper around a <see cref="IMdmEntity" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMdmModelEntity<T>
        where T : class, IMdmEntity
    {
        /// <summary>
        /// Gets or sets the model source, the underlying MDM contract.
        /// </summary>
        T Source { get; set; }
    }
}