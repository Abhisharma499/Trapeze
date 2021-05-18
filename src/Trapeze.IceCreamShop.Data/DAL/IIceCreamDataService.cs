namespace Trapeze.IceCreamShop.Data.DAL
{
    using System.Threading.Tasks;
    using Trapeze.IceCreamShop.Data.Entities;

    public interface IIceCreamDataService
    {
        Task<PurchaseInformation> PurchaseIceCream(IceCreamInformation iceCreamModel);
    }
}
