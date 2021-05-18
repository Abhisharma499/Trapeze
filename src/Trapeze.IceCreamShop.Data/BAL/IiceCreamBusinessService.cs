namespace Trapeze.IceCreamShop.Data.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Trapeze.IceCreamShop.Data.Entities;
    using Trapeze.IceCreamShop.Enums;

    public interface IIceCreamBusinessService
    {
        Task<decimal> GetTotalCost(IceCreamInformation iceCreamModel);

        Task<decimal> CalculateBaseCost(IceCreamBase iceCreamBase);

        Task<decimal> CalculateFlavoursCost(ICollection<FlavourInformation> flavours);

        bool ValidateNumberOfScoops(ICollection<FlavourInformation> flavours);

        Task<bool> ValidatePurChaseTime(DateTime puchaseTime);

        Task<PurchaseInformation> PurchaseIceCream(IceCreamInformation iceCreamModel);

        Task<bool> ValidatePurchaseAmount(decimal userPurchaseAmount, decimal calculatedPurchaseAmount);

        Task<bool> CheckIfIceCreamBaseValid(IceCreamBase iceCreamBase);

        Task<decimal> CalculateRefund(decimal userPurchaseAmount, decimal calculatedPurchaseAmount);

        Task<bool> CheckIfIceCreamFlavourValid(IceCreamFlavour iceCreamFlavour);

        Task<bool> ValidateIceCreamAndFlavourCombination(IReadOnlyCollection<FlavourInformation> flavours, IceCreamBase iceCreamBase);

        Task<bool> ValidateCookieDoughflavourtheSugarConeBase(FlavourInformation flavour, IceCreamBase iceCreamBase); Task<bool> ValidateFlavourCombination(IReadOnlyCollection<FlavourInformation> allVailableflavours, IReadOnlyCollection<FlavourInformation> selectedFlavours);
    }
}
