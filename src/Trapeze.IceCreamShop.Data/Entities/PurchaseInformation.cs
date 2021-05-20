namespace Trapeze.IceCreamShop.Data.Entities
{
    using System.Text.Json.Serialization;
    using Trapeze.IceCreamShop.Enums;

    public class PurchaseInformation
    {
        public decimal Refund { get; set; }

        [JsonIgnore]
        public PurchaseStates State { get; set; }

        [JsonIgnore]
        public IceCreamInformation IceCreamOrder { get; set; }

        [JsonIgnore]
        public decimal CostOfIceCream { get; set; }
    }
}
