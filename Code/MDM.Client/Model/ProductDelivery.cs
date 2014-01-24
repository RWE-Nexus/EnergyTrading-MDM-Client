namespace EnergyTrading.MDM.Client.Model
{
    using System.Collections.Generic;

    public class ProductDelivery : IMdmModelEntity<RWEST.Nexus.MDM.Contracts.ProductDelivery>
    {
        private Model.Product product;
        private string deliveryRange;

        public RWEST.Nexus.MDM.Contracts.ProductDelivery Source { get; set; }

        public string Name
        {
            get 
            {
                if (this.ProductTypeInstance != null)
                {
                    return this.ProductTypeInstance.Source.Details.Name;
                }

                if (this.ProductType != null)
                {
                    return this.ProductType.Source.Details.Name;
                }

                if (this.Product != null)
                {
                    return this.Product.Source.Details.Name;
                }

                return null;
            }
        }

        public string DeliveryRange
        {
            get
            {
                if (!string.IsNullOrEmpty(this.deliveryRange))
                {
                    return this.deliveryRange;
                }

                if (this.ProductTypeInstance != null)
                {
                    return this.ProductTypeInstance.ProductType.Source.Details.DeliveryRangeType;
                }

                return this.ProductType != null
                    ? this.ProductType.Source.Details.DeliveryRangeType
                    : null;
            }
            set
            {
                this.deliveryRange = value;
            }
        }

        public Model.ProductType ProductType { get; set; }

        public Model.ProductTypeInstance ProductTypeInstance { get; set; }

        public Model.Product Product
        {
            get
            {
                if (this.ProductTypeInstance != null)
                {
                    return this.ProductTypeInstance.ProductType.Product;
                }

                if (this.ProductType != null)
                {
                    return this.ProductType.Product;
                }

                return this.product;
            }
            set
            {
                this.product = value;
            }
        }

        public List<InstrumentTypeOverride> InstrumentTypeOverrides
        {
            get
            {
                if (this.ProductTypeInstance != null)
                {
                    return this.ProductTypeInstance.ProductType.InstrumentTypeOverrides;
                }

                return this.ProductType != null
                    ? this.ProductType.InstrumentTypeOverrides
                    : new List<InstrumentTypeOverride>();
            }
        }
    }
}