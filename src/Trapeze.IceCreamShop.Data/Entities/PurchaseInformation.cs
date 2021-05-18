using System.Text.Json.Serialization;
using Trapeze.IceCreamShop.Enums;

namespace Trapeze.IceCreamShop.Data.Entities
{
    public class PurchaseInformation
    {
        public decimal Refund { get; set; }

        [JsonIgnore]
        public bool IsSuccess { get; set; } = false;

        [JsonIgnore]
        public PurchaseStates State { get; set; }
    }
}
