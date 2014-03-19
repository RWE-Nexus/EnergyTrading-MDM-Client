namespace MDM.Client.IntegrationTests
{
    using System;

    using EnergyTrading.Mdm.Contracts;

    public class ObjectMother
    {
        public static T Create<T>()
            where T : IMdmEntity
        {
            var value = Create(typeof(T).Name);

            return (T)value;
        }

        public static IMdmEntity Create(string name)
        {
            switch (name)
            {
                case "SourceSystem":
                    return CreateSourceSystem();
                default:
                    throw new NotImplementedException("Unsupported MDM Entity type: " + name);
            }
        }

        private static IMdmEntity CreateSourceSystem()
        {
            var guid = Guid.NewGuid();

            return new EnergyTrading.Mdm.Contracts.SourceSystem
            {
                Identifiers = CreateIdList(guid),
                Details = new SourceSystemDetails
                              {
                                  Name = "SourceSystem" + Guid.NewGuid()
                              },
            };
        }

        private static MdmIdList CreateIdList(Guid guid)
        {
            return new MdmIdList
                       {
                           new MdmId { SystemName = "Endur", Identifier = "Endur" + guid },
                           new MdmId { SystemName = "Trayport", Identifier = "Trayport" + guid },
                       };
        }
    }
}