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
        private readonly DAL.IIceCreamDataService _iceCreamDataService;

        public IceCreamBusinessService(DAL.IIceCreamDataService iceCreamDataService)
        {
            _iceCreamDataService = iceCreamDataService;
        }

        public decimal CalculateBaseCost(IceCreamBase iceCreamBase)
        {
            try
            {
                if (iceCreamBase == IceCreamBase.WaffleCone)
                {
                    return Constants.PriceWaffleCone;
                }

                return Constants.PriceNonWaffleCone;
            }
            catch
            {
                throw;
            }
        }

        public decimal CalculateFlavoursCost(ICollection<FlavourInformation> flavours)
        {
            try
            {
                switch (flavours?.Count)
                {
                    case 1:
                        return 2;
                    case 2:
                        return 3;
                    case 3:
                        return 3.5M;
                    case 4:
                        return 3.8M;
                    default:
                        return 0;
                }
            }
            catch
            {
                throw;
            }
        }

        public bool ValidateNumberOfScoops(ICollection<FlavourInformation> flavours)
        {
            try
            {
                if (flavours == null)
                {
                    return false;
                }

                if (flavours.Count >= Constants.MinFlavourCountAllowed && flavours.Count <= Constants.MaxFlavourCountAllowed)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                throw;
            }
        }

        public bool ValidatePurchaseTime(DateTime purchaseTime)
        {
            try
            {
                bool isDayValid = Constants.AllowedDaysForPurchase.Contains(purchaseTime.DayOfWeek);

                bool isTimeValid = purchaseTime.TimeOfDay >= Constants.StartTimeOfStore && purchaseTime.TimeOfDay <= Constants.EndTimeOfStore;

                return isDayValid && isTimeValid;
            }
            catch
            {
                throw;
            }
        }

        public async Task<PurchaseInformation> PurchaseIceCream(IceCreamInformation iceCreamModel)
        {
            PurchaseInformation purchaseSucessViewModel = new PurchaseInformation();

            purchaseSucessViewModel.IceCreamOrder = iceCreamModel;

            if (iceCreamModel == null)
            {
                return null;
            }

            try
            {
                // The ice cream store is only open on Tuesday - Saturday from 9:00 AM to 5:45 PM.
                if (!ValidatePurchaseTime(DateTime.Now))
                {
                    purchaseSucessViewModel.State = PurchaseStates.TimeInvalid;
                    return purchaseSucessViewModel;
                }

                // The ice cream base must be from the list of available bases.
                if (!CheckIfIceCreamBaseValid(iceCreamModel.Base.IceCreamBase))
                {
                    purchaseSucessViewModel.State = PurchaseStates.BaseInvalid;
                    return purchaseSucessViewModel;
                }

                // The ice cream flavours must be from the list of available flavours.
                foreach (var flavour in iceCreamModel.Flavours)
                {
                    if (!CheckIfIceCreamFlavourValid(flavour.IceCreamFlavour))
                    {
                        purchaseSucessViewModel.State = PurchaseStates.FlavourInvalid;
                        return purchaseSucessViewModel;
                    }
                }

                // Max scoops can be 4
                if (!ValidateNumberOfScoops(iceCreamModel.Flavours))
                {
                    purchaseSucessViewModel.State = PurchaseStates.NumberOfScoopsInvalid;
                    return purchaseSucessViewModel;
                }

                // Only a cup of ice cream can have 4 scoops.
                if (!ValidateIceCreamAndFlavourCombination(iceCreamModel.Flavours, iceCreamModel.Base.IceCreamBase))
                {
                    purchaseSucessViewModel.State = PurchaseStates.BaseFlavourCombinationInvalid;
                    return purchaseSucessViewModel;
                }

                // We will not give Cookie Dough flavour in the Sugar Cone base.
                foreach (var flavour in iceCreamModel.Flavours)
                {
                    if (!ValidateCookieDoughflavourtheSugarConeBase(flavour, iceCreamModel.Base.IceCreamBase))
                    {
                        purchaseSucessViewModel.State = PurchaseStates.BaseFlavourCombinationInvalid;
                        return purchaseSucessViewModel;
                    }
                }

                // Validate We will not give Strawberry and Mint Chocolate Chip flavours together.
                if (!ValidateFlavourCombination(iceCreamModel.Flavours.Select(x => x.IceCreamFlavour).ToList(), Constants.FlavoursRestrictedTogether))
                {
                    purchaseSucessViewModel.State = PurchaseStates.FlavourCombinationInvalid;
                    return purchaseSucessViewModel;
                }

                // We will give any combination of two of the above though. I.e.Cookies And Cream and Vanilla are ok together.
                // The only exception to the rule above is if the base is a Cake Cone.
                if (!(iceCreamModel.Base.IceCreamBase == IceCreamBase.CakeCone))
                {
                    List<IceCreamFlavour> flavours = new List<IceCreamFlavour>();

                    flavours = iceCreamModel.Flavours.Select(x => x.IceCreamFlavour).ToList();

                    // Validate We will not give Cookies And Cream, Moose Tracks, and Vanilla together.
                    if (!ValidateFlavourCombination(flavours, Constants.FlavoursRestrictedTogether2))
                    {
                        purchaseSucessViewModel.State = PurchaseStates.FlavourCombinationInvalid;
                        return purchaseSucessViewModel;
                    }
                }

                decimal baseCost = CalculateBaseCost(iceCreamModel.Base.IceCreamBase);

                decimal flavourCost = CalculateFlavoursCost(iceCreamModel.Flavours);

                // The amount passed in to purchase must match or be greater than the price
                if (!ValidatePurchaseAmount(iceCreamModel.PurchaseAmount, baseCost + flavourCost))
                {
                    purchaseSucessViewModel.State = PurchaseStates.AmountLessThanCost;
                    purchaseSucessViewModel.CostOfIceCream = baseCost + flavourCost;
                    return purchaseSucessViewModel;
                }

                decimal? refundAmount = CalculateRefund(iceCreamModel.PurchaseAmount, baseCost + flavourCost);

                if (refundAmount == null)
                {
                    purchaseSucessViewModel.State = PurchaseStates.RefundAmountInvalid;
                    return purchaseSucessViewModel;
                }
                else
                {
                    purchaseSucessViewModel.Refund = refundAmount.Value;
                }

                bool isInserted = await _iceCreamDataService.InsertIntoIceCreamInformation(iceCreamModel).ConfigureAwait(false);

                if (isInserted)
                {
                    purchaseSucessViewModel.State = PurchaseStates.Success;
                }
                else
                {
                    purchaseSucessViewModel.State = PurchaseStates.DataBaseInsertionError;
                }

                return await Task.FromResult(purchaseSucessViewModel).ConfigureAwait(false);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ValidatePurchaseAmount(decimal userPurchaseAmount, decimal calculatedPurchaseAmount)
        {
            return userPurchaseAmount >= calculatedPurchaseAmount;
        }

        public bool CheckIfIceCreamBaseValid(IceCreamBase iceCreamBase)
        {
            return Enum.IsDefined(typeof(IceCreamBase), iceCreamBase);
        }

        public decimal? CalculateRefund(decimal userPurchaseAmount, decimal calculatedPurchaseAmount)
        {
            try
            {
                decimal refund = userPurchaseAmount - calculatedPurchaseAmount;

                if (refund < 0)
                {
                    return null;
                }

                return userPurchaseAmount - calculatedPurchaseAmount;
            }
            catch
            {
                throw;
            }
        }

        public bool CheckIfIceCreamFlavourValid(IceCreamFlavour iceCreamFlavour)
        {
            return Enum.IsDefined(typeof(IceCreamFlavour), iceCreamFlavour);
        }

        public bool ValidateFlavourCombination(ICollection<IceCreamFlavour> selectediceCreamFlavours, ICollection<IceCreamFlavour> restrictedIceCreamFlavours)
        {
            try
            {
                if (selectediceCreamFlavours == null)
                {
                    return false;
                }

                List<bool> flavourPresent = new List<bool>();

                foreach (IceCreamFlavour flavour in restrictedIceCreamFlavours)
                {
                    flavourPresent.Add(selectediceCreamFlavours.Contains(flavour));
                }

                return !flavourPresent.All(x => x == true);
            }
            catch
            {
                throw;
            }
        }

        public bool ValidateIceCreamAndFlavourCombination(ICollection<FlavourInformation> flavours, IceCreamBase iceCreamBase)
        {
            try
            {
                if (iceCreamBase != IceCreamBase.Cup && flavours?.Count >= 4)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

        public bool ValidateCookieDoughflavourtheSugarConeBase(FlavourInformation flavour, IceCreamBase iceCreamBase)
        {
            try
            {
                if (iceCreamBase == IceCreamBase.SugarCone && flavour?.IceCreamFlavour == IceCreamFlavour.CookieDough)
                {
                    return false;
                }

                return true;
            }
            catch
            {
                throw;
            }
        }

        public string GetErrorMessageByPurchaseState(PurchaseInformation purchase)
        {
            switch (purchase?.State)
            {
                case PurchaseStates.AmountInvalid:
                    return $"Invalid purchaseAmount!. Please check the purchase Amount {purchase.IceCreamOrder.PurchaseAmount}";
                case PurchaseStates.AmountLessThanCost:
                    return $"Invalid purchaseAmount!. Purchase Amount {purchase.IceCreamOrder.PurchaseAmount} must be greater than the cost amount {purchase.CostOfIceCream}.";
                case PurchaseStates.TimeInvalid:
                    return $"Invalid purchase time. Please visit between store hours.";
                case PurchaseStates.BaseInvalid:
                    return $"Invalid ice cream base. Please select a valid base.";
                case PurchaseStates.FlavourInvalid:
                    return $"Invalid ice cream flavour. Please select a valid flavour.";
                case PurchaseStates.NumberOfScoopsInvalid:
                    return $"Invalid number of scoops. Please select valid number of scoops";
                case PurchaseStates.BaseFlavourCombinationInvalid:
                    return $"Invalid base and flavour combination. Please select a different combination";
                case PurchaseStates.FlavourCombinationInvalid:
                    return $"Invalid flavour combination. Please select a different combination";
                default:
                    return string.Empty;
            }
        }
    }
}
