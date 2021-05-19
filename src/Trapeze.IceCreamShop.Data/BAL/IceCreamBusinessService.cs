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

        private readonly DAL.IIceCreamDataService _iceCreamDataService;

        public IceCreamBusinessService(IceCreamDbContext ctx, DAL.IIceCreamDataService iceCreamDataService)
        {
            _context = ctx;
            _iceCreamDataService = iceCreamDataService;
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

        public async Task<bool> ValidateNumberOfScoops(ICollection<FlavourInformation> flavours)
        {
            if (flavours == null)
            {
                return await Task.FromResult(false).ConfigureAwait(false);
            }

            if (flavours.Count >= Constants.MinFlavourCountAllowed && flavours.Count <= Constants.MaxFlavourCountAllowed)
            {
                return await Task.FromResult(true).ConfigureAwait(false);
            }

            return await Task.FromResult(false).ConfigureAwait(false);
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
                if (!await ValidatePurChaseTime(iceCreamModel.PurchaseDateTime).ConfigureAwait(false))
                {
                    purchaseSucessViewModel.IsSuccess = false;
                    purchaseSucessViewModel.State = PurchaseStates.PurchaseTimeInvalid;
                    return purchaseSucessViewModel;
                }

                if (!await CheckIfIceCreamBaseValid(iceCreamModel.Base.IceCreamBase).ConfigureAwait(true))
                {
                    purchaseSucessViewModel.IsSuccess = false;
                    purchaseSucessViewModel.State = PurchaseStates.PurchaseBaseInvalid;
                    return purchaseSucessViewModel;
                }

                foreach (var flavour in iceCreamModel.Flavours)
                {
                    if (!await CheckIfIceCreamFlavourValid(flavour.IceCreamFlavour).ConfigureAwait(true))
                    {
                        purchaseSucessViewModel.IsSuccess = false;
                        purchaseSucessViewModel.State = PurchaseStates.PurchaseBaseInvalid;
                        return purchaseSucessViewModel;
                    }
                }

                if (!await ValidateNumberOfScoops(iceCreamModel.Flavours).ConfigureAwait(false))
                {
                    purchaseSucessViewModel.IsSuccess = false;
                    purchaseSucessViewModel.State = PurchaseStates.PurchaseNumberOfScoopsInvalid;
                    return purchaseSucessViewModel;
                }

                // Validate Only a cup of ice cream can have 4 scoops.
                if (!await ValidateIceCreamAndFlavourCombination(iceCreamModel.Flavours, iceCreamModel.Base.IceCreamBase).ConfigureAwait(false))
                {
                    purchaseSucessViewModel.IsSuccess = false;
                    purchaseSucessViewModel.State = PurchaseStates.PurchaseBaseFlavourCombinationInvalid;
                    return purchaseSucessViewModel;
                }

                // Validate ValidateCookieDoughflavourtheSugarConeBase
                foreach (var flavour in iceCreamModel.Flavours)
                {
                    if (!await ValidateCookieDoughflavourtheSugarConeBase(flavour, iceCreamModel.Base.IceCreamBase).ConfigureAwait(false))
                    {
                        purchaseSucessViewModel.IsSuccess = false;
                        purchaseSucessViewModel.State = PurchaseStates.PurchaseBaseFlavourCombinationInvalid;
                        return purchaseSucessViewModel;
                    }
                }

                // We will not give Strawberry and Mint Chocolate Chip flavours together.
                // We will not give Cookies And Cream, Moose Tracks, and Vanilla together.
                // We will give any combination of two of the above though. I.e.Cookies And Cream and Vanilla are ok together.
                // The only exception to the rule above is if the base is a Cake Cone.
                if (!(iceCreamModel.Base.IceCreamBase == IceCreamBase.CakeCone))
                {
                    List<IceCreamFlavour> flavours = new List<IceCreamFlavour>();

                    flavours = iceCreamModel.Flavours.Select(x => x.IceCreamFlavour).ToList();

                    // Validate We will not give Strawberry and Mint Chocolate Chip flavours together.
                    if (!await ValidateFlavourCombination(flavours, Constants.FlavoursRestrictedTogether).ConfigureAwait(false))
                    {
                        purchaseSucessViewModel.IsSuccess = false;
                        purchaseSucessViewModel.State = PurchaseStates.PurchaseFlavourCombinationInvalid;
                        return purchaseSucessViewModel;
                    }

                    // Validate We will not give Cookies And Cream, Moose Tracks, and Vanilla together.
                    if (!await ValidateFlavourCombination(flavours, Constants.FlavoursRestrictedTogether2).ConfigureAwait(false))
                    {
                        purchaseSucessViewModel.IsSuccess = false;
                        purchaseSucessViewModel.State = PurchaseStates.PurchaseFlavourCombinationInvalid;
                        return purchaseSucessViewModel;
                    }
                }

                decimal baseCost = await CalculateBaseCost(iceCreamModel.Base.IceCreamBase).ConfigureAwait(false);

                decimal flavourCost = await CalculateFlavoursCost(iceCreamModel.Flavours).ConfigureAwait(false);

                if (!await ValidatePurchaseAmount(iceCreamModel.PurchaseAmount, baseCost + flavourCost).ConfigureAwait(false))
                {
                    purchaseSucessViewModel.IsSuccess = false;
                    purchaseSucessViewModel.State = PurchaseStates.PurchaseAmountInvalid;
                    return purchaseSucessViewModel;
                }

                purchaseSucessViewModel.Refund = await CalculateRefund(iceCreamModel.PurchaseAmount, baseCost + flavourCost).ConfigureAwait(false);

                bool isInserted = await _iceCreamDataService.InsertIntoIceCreamInformation(iceCreamModel).ConfigureAwait(false);

                if (isInserted)
                {
                    purchaseSucessViewModel.IsSuccess = true;
                }

                return await Task.FromResult(purchaseSucessViewModel).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
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

        public async Task<bool> ValidateFlavourCombination(ICollection<IceCreamFlavour> selectediceCreamFlavours, ICollection<IceCreamFlavour> restrictedIceCreamFlavours)
        {
            if (selectediceCreamFlavours == null)
            {
                return await Task.FromResult(false).ConfigureAwait(false);
            }

            List<bool> flavourPresent = new List<bool>();

            foreach (IceCreamFlavour flavour in restrictedIceCreamFlavours)
            {
                flavourPresent.Add(selectediceCreamFlavours.Contains(flavour));
            }

            return await Task.FromResult(!flavourPresent.All(x => x == true)).ConfigureAwait(false);
        }

        public async Task<bool> ValidateIceCreamAndFlavourCombination(ICollection<FlavourInformation> flavours, IceCreamBase iceCreamBase)
        {
            if (iceCreamBase != IceCreamBase.Cup && flavours?.Count >= 4)
            {
                return await Task.FromResult(false).ConfigureAwait(false);
            }

            return await Task.FromResult(true).ConfigureAwait(false);
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
