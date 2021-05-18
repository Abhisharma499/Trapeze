namespace Trapeze.IceCreamShop.Data.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Trapeze.IceCreamShop.Data.Entities;
    using Trapeze.IceCreamShop.Enums;

    public class IceCreamBusinessService : IIceCreamBusinessService
    {
        private readonly IceCreamDbContext _context;

        public IceCreamBusinessService(IceCreamDbContext ctx)
        {
            _context = ctx;
        }

        public async Task<decimal> GetTotalCost(IceCreamInformation iceCreamModel)
        {
            try
            {
                if (iceCreamModel == null)
                {
                    throw new Exception("The Ice cream model cannot be null");
                }
                else
                {
                    decimal baseCost = await CalculateBaseCost(iceCreamModel.Base.IceCreamBase).ConfigureAwait(false);
                    decimal flavourCost = await CalculateFlavoursCost(iceCreamModel.Flavours.ToList()).ConfigureAwait(false);

                    return baseCost + flavourCost;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<decimal> CalculateBaseCost(IceCreamBase iceCreamBase)
        {
            if (iceCreamBase == Enums.IceCreamBase.WaffleCone)
            {
                return await Task.FromResult(Constants.PriceWaffleCone).ConfigureAwait(false);
            }
            else
            {
                return await Task.FromResult(Constants.PriceNonWaffleCone).ConfigureAwait(false);
            }
        }

        public async Task<decimal> CalculateFlavoursCost(ICollection<FlavourInformation> flavours)
        {
            try
            {
                if (flavours == null)
                {
                    throw new Exception("The Ice cream model cannot be null");
                }

                if (ValidateNumberOfScoops(flavours))
                {
                    throw new Exception("The number of Flavours must be between 1 to 4.");
                }

                switch (flavours.Count)
                {
                    case 1:
                        return await Task.FromResult(2).ConfigureAwait(false);
                    case 2:
                        return await Task.FromResult(3).ConfigureAwait(false);
                    case 3:
                        return await Task.FromResult(3.5M).ConfigureAwait(false);
                    case 4:
                        return await Task.FromResult(3.8M).ConfigureAwait(false);
                    default:
                        throw new Exception("Maximum flavour count exceeded");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ValidateNumberOfScoops(ICollection<FlavourInformation> flavours)
        {
            if (flavours == null)
            {
                return false;
            }

            if (flavours.Count >= Constants.MinFlavourCountAllowed && flavours.Count < Constants.MaxFlavourCountAllowed)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> ValidatePurChaseTime(DateTime puchaseTime)
        {
            try
            {
                bool isDayValid = Constants.AllowedDaysForPurchase.Contains(puchaseTime.DayOfWeek);

                bool isTimeValid = puchaseTime.TimeOfDay >= Constants.StartTimeOfStore && puchaseTime.TimeOfDay <= Constants.EndTimeOfStore;

                return await Task.FromResult(isDayValid && isDayValid).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid purchase time. Please try in store hours.");
            }
        }

        public async Task<PurchaseInformation> PurchaseIceCream(IceCreamInformation iceCreamModel)
        {
            PurchaseInformation purchaseSucessViewModel = new PurchaseInformation();

            if (iceCreamModel == null)
            {
                return null;
            }

            try
            {
                if (await ValidatePurChaseTime(iceCreamModel.PurchaseDateTime).ConfigureAwait(true) && await CheckIfIceCreamBaseValid(iceCreamModel.Base.IceCreamBase).ConfigureAwait(true))
                {
                    decimal baseCost = await CalculateBaseCost(iceCreamModel.Base.IceCreamBase).ConfigureAwait(false);

                    decimal flavourCost = await CalculateFlavoursCost(iceCreamModel.Flavours).ConfigureAwait(false);

                    if (await ValidatePurchaseAmount(iceCreamModel.PurchaseAmount, baseCost + flavourCost).ConfigureAwait(true))
                    {
                        decimal refund = await CalculateRefund(iceCreamModel.PurchaseAmount, baseCost + flavourCost).ConfigureAwait(false);
                        purchaseSucessViewModel.Refund = refund;
                    }

                    bool isInserted = await InsertIntoIceCreamInformation(iceCreamModel).ConfigureAwait(false);

                    if (isInserted)
                    {
                        purchaseSucessViewModel.IsSuccess = true;
                    }

                    return await Task.FromResult(purchaseSucessViewModel).ConfigureAwait(false);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return await Task.FromResult(purchaseSucessViewModel).ConfigureAwait(false);
        }

        public async Task<bool> ValidatePurchaseAmount(decimal userPurchaseAmount, decimal calculatedPurchaseAmount)
        {
            try
            {
                if (userPurchaseAmount >= calculatedPurchaseAmount)
                {
                    return await Task.FromResult(true).ConfigureAwait(false);
                }
                else
                {
                    throw new Exception($"The purchase Amount {userPurchaseAmount} must be greater than equal to cost of ice cream {calculatedPurchaseAmount}.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> CheckIfIceCreamBaseValid(IceCreamBase iceCreamBase)
        {
            return await Task.FromResult(Enum.IsDefined(typeof(IceCreamBase), iceCreamBase)).ConfigureAwait(false);
        }

        public async Task<decimal> CalculateRefund(decimal userPurchaseAmount, decimal calculatedPurchaseAmount)
        {
            try
            {
                decimal refund = userPurchaseAmount - calculatedPurchaseAmount;

                if (refund < 0)
                {
                    throw new Exception("Refund cannot be negative");
                }

                return await Task.FromResult(userPurchaseAmount - calculatedPurchaseAmount).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> CheckIfIceCreamFlavourValid(IceCreamFlavour iceCreamFlavour)
        {
            return await Task.FromResult(Enum.IsDefined(typeof(IceCreamFlavour), iceCreamFlavour)).ConfigureAwait(false);
        }

        public async Task<bool> ValidateFlavourCombination(IReadOnlyCollection<FlavourInformation> allVailableflavours, IReadOnlyCollection<FlavourInformation> selectedFlavours)
        {
            List<bool> flavourPresent = new List<bool>();

            foreach (FlavourInformation fl in selectedFlavours)
            {
                if (allVailableflavours.Select(x => x.IceCreamFlavour).ToList().Contains(fl.IceCreamFlavour))
                {
                    flavourPresent.Add(true);
                }
                else
                {
                    flavourPresent.Add(false);
                }
            }

            return await Task.FromResult(flavourPresent.Any(x => x == false)).ConfigureAwait(false);
        }

        public async Task<bool> ValidateIceCreamAndFlavourCombination(IReadOnlyCollection<FlavourInformation> flavours, IceCreamBase iceCreamBase)
        {
            if (iceCreamBase != IceCreamBase.Cup && flavours?.Count >= 4)
            {
                return await Task.FromResult(true).ConfigureAwait(false);
            }

            return await Task.FromResult(false).ConfigureAwait(false);
        }

        public async Task<bool> ValidateCookieDoughflavourtheSugarConeBase(FlavourInformation flavour, IceCreamBase iceCreamBase)
        {
            if (iceCreamBase == IceCreamBase.SugarCone && flavour?.IceCreamFlavour == IceCreamFlavour.CookieDough)
            {
                return await Task.FromResult(false).ConfigureAwait(false);
            }

            return await Task.FromResult(true).ConfigureAwait(false);
        }
    }
}
