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

        decimal CalculateBaseCost(IceCreamBase iceCreamBase);

        decimal CalculateFlavoursCost(ICollection<FlavourInformation> flavours);

        bool ValidateNumberOfScoops(ICollection<FlavourInformation> flavours);

        bool ValidatePurchaseTime(DateTime puchaseTime);

        bool ValidatePurchaseAmount(decimal userPurchaseAmount, decimal calculatedPurchaseAmount);

        bool CheckIfIceCreamBaseValid(IceCreamBase iceCreamBase);

        decimal? CalculateRefund(decimal userPurchaseAmount, decimal calculatedPurchaseAmount);

        bool CheckIfIceCreamFlavourValid(IceCreamFlavour iceCreamFlavour);

        bool ValidateIceCreamAndFlavourCombination(ICollection<FlavourInformation> flavours, IceCreamBase iceCreamBase);

        bool ValidateCookieDoughflavourtheSugarConeBase(FlavourInformation flavour, IceCreamBase iceCreamBase);

        bool ValidateFlavourCombination(ICollection<IceCreamFlavour> allVailableflavours, ICollection<IceCreamFlavour> selectedFlavours);

        string GetErrorMessageByPurchaseState(PurchaseInformation purchaseState);
    }
}
