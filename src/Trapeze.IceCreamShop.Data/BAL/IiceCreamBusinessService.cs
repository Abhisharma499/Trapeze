namespace Trapeze.IceCreamShop.Data.BAL
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Trapeze.IceCreamShop.Data.Entities;
    using Trapeze.IceCreamShop.Enums;

    public interface IIceCreamBusinessService
    {
        Task<PurchaseInformation> PurchaseIceCream(IceCreamInformation iceCreamModel);

        Task<decimal> CalculateBaseCost(IceCreamBase iceCreamBase);

        Task<decimal> CalculateFlavoursCost(ICollection<FlavourInformation> flavours);

        Task<bool> ValidateNumberOfScoops(ICollection<FlavourInformation> flavours);

        Task<bool> ValidatePurChaseTime(DateTime puchaseTime);

        Task<bool> ValidatePurchaseAmount(decimal userPurchaseAmount, decimal calculatedPurchaseAmount);

        Task<bool> CheckIfIceCreamBaseValid(IceCreamBase iceCreamBase);

        Task<decimal?> CalculateRefund(decimal userPurchaseAmount, decimal calculatedPurchaseAmount);

        Task<bool> CheckIfIceCreamFlavourValid(IceCreamFlavour iceCreamFlavour);

        Task<bool> ValidateIceCreamAndFlavourCombination(ICollection<FlavourInformation> flavours, IceCreamBase iceCreamBase);

        Task<bool> ValidateCookieDoughflavourtheSugarConeBase(FlavourInformation flavour, IceCreamBase iceCreamBase);

        Task<bool> ValidateFlavourCombination(ICollection<IceCreamFlavour> allVailableflavours, ICollection<IceCreamFlavour> selectedFlavours);

        Task<string> HandleErrorState(PurchaseInformation purchaseState);

        Task<string> GetUserName(HttpContext httpContext);
    }
}
