namespace Trapeze.IceCreamShop.Data.BAL
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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

        public async Task<decimal> CalculateBaseCost(IceCreamBase iceCreamBase)
        {
            try
            {
                if (iceCreamBase == IceCreamBase.WaffleCone)
                {
                    return await Task.FromResult(Constants.PriceWaffleCone).ConfigureAwait(false);
                }
                else
                {
                    return await Task.FromResult(Constants.PriceNonWaffleCone).ConfigureAwait(false);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<decimal> CalculateFlavoursCost(ICollection<FlavourInformation> flavours)
        {
            try
            {
                switch (flavours?.Count)
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
                        return await Task.FromResult(0).ConfigureAwait(false);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> ValidateNumberOfScoops(ICollection<FlavourInformation> flavours)
        {
            try
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
            catch
            {
                throw;
            }
        }

        public async Task<bool> ValidatePurChaseTime(DateTime puchaseTime)
        {
            try
            {
                bool isDayValid = Constants.AllowedDaysForPurchase.Contains(puchaseTime.DayOfWeek);

                bool isTimeValid = puchaseTime.TimeOfDay >= Constants.StartTimeOfStore && puchaseTime.TimeOfDay <= Constants.EndTimeOfStore;

                return await Task.FromResult(isDayValid && isTimeValid).ConfigureAwait(false);
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
                if (!await ValidatePurChaseTime(DateTime.Now).ConfigureAwait(false))
                {
                    purchaseSucessViewModel.IsSuccess = false;
                    purchaseSucessViewModel.State = PurchaseStates.TimeInvalid;
                    return purchaseSucessViewModel;
                }

                // The ice cream base must be from the list of available bases.
                if (!await CheckIfIceCreamBaseValid(iceCreamModel.Base.IceCreamBase).ConfigureAwait(true))
                {
                    purchaseSucessViewModel.IsSuccess = false;
                    purchaseSucessViewModel.State = PurchaseStates.BaseInvalid;
                    return purchaseSucessViewModel;
                }

                // The ice cream flavours must be from the list of available flavours.
                foreach (var flavour in iceCreamModel.Flavours)
                {
                    if (!await CheckIfIceCreamFlavourValid(flavour.IceCreamFlavour).ConfigureAwait(true))
                    {
                        purchaseSucessViewModel.IsSuccess = false;
                        purchaseSucessViewModel.State = PurchaseStates.FlavourInvalid;
                        return purchaseSucessViewModel;
                    }
                }

                // Max scoops can be 4
                if (!await ValidateNumberOfScoops(iceCreamModel.Flavours).ConfigureAwait(false))
                {
                    purchaseSucessViewModel.IsSuccess = false;
                    purchaseSucessViewModel.State = PurchaseStates.NumberOfScoopsInvalid;
                    return purchaseSucessViewModel;
                }

                // Only a cup of ice cream can have 4 scoops.
                if (!await ValidateIceCreamAndFlavourCombination(iceCreamModel.Flavours, iceCreamModel.Base.IceCreamBase).ConfigureAwait(false))
                {
                    purchaseSucessViewModel.IsSuccess = false;
                    purchaseSucessViewModel.State = PurchaseStates.BaseFlavourCombinationInvalid;
                    return purchaseSucessViewModel;
                }

                // We will not give Cookie Dough flavour in the Sugar Cone base.
                foreach (var flavour in iceCreamModel.Flavours)
                {
                    if (!await ValidateCookieDoughflavourtheSugarConeBase(flavour, iceCreamModel.Base.IceCreamBase).ConfigureAwait(false))
                    {
                        purchaseSucessViewModel.IsSuccess = false;
                        purchaseSucessViewModel.State = PurchaseStates.BaseFlavourCombinationInvalid;
                        return purchaseSucessViewModel;
                    }
                }

                // Validate We will not give Strawberry and Mint Chocolate Chip flavours together.
                if (!await ValidateFlavourCombination(iceCreamModel.Flavours.Select(x => x.IceCreamFlavour).ToList(), Constants.FlavoursRestrictedTogether).ConfigureAwait(false))
                {
                    purchaseSucessViewModel.IsSuccess = false;
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
                    if (!await ValidateFlavourCombination(flavours, Constants.FlavoursRestrictedTogether2).ConfigureAwait(false))
                    {
                        purchaseSucessViewModel.IsSuccess = false;
                        purchaseSucessViewModel.State = PurchaseStates.FlavourCombinationInvalid;
                        return purchaseSucessViewModel;
                    }
                }

                decimal baseCost = await CalculateBaseCost(iceCreamModel.Base.IceCreamBase).ConfigureAwait(false);

                decimal flavourCost = await CalculateFlavoursCost(iceCreamModel.Flavours).ConfigureAwait(false);

                // The amount passed in to purchase must match or be greater than the price
                if (!await ValidatePurchaseAmount(iceCreamModel.PurchaseAmount, baseCost + flavourCost).ConfigureAwait(false))
                {
                    purchaseSucessViewModel.IsSuccess = false;
                    purchaseSucessViewModel.State = PurchaseStates.AmountLessThanCost;
                    purchaseSucessViewModel.CostOfIceCream = baseCost + flavourCost;
                    return purchaseSucessViewModel;
                }

                decimal? refundAmount = await CalculateRefund(iceCreamModel.PurchaseAmount, baseCost + flavourCost).ConfigureAwait(false);

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
                    purchaseSucessViewModel.IsSuccess = true;
                    purchaseSucessViewModel.State = PurchaseStates.Sucess;
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
                    return await Task.FromResult(false).ConfigureAwait(false);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> CheckIfIceCreamBaseValid(IceCreamBase iceCreamBase)
        {
            try
            {
                return await Task.FromResult(Enum.IsDefined(typeof(IceCreamBase), iceCreamBase)).ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
        }

        public async Task<decimal?> CalculateRefund(decimal userPurchaseAmount, decimal calculatedPurchaseAmount)
        {
            try
            {
                decimal refund = userPurchaseAmount - calculatedPurchaseAmount;

                if (refund < 0)
                {
                    return null;
                }

                return await Task.FromResult(userPurchaseAmount - calculatedPurchaseAmount).ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> CheckIfIceCreamFlavourValid(IceCreamFlavour iceCreamFlavour)
        {
            try
            {
                return await Task.FromResult(Enum.IsDefined(typeof(IceCreamFlavour), iceCreamFlavour)).ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> ValidateFlavourCombination(ICollection<IceCreamFlavour> selectediceCreamFlavours, ICollection<IceCreamFlavour> restrictedIceCreamFlavours)
        {
            try
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
            catch
            {
                throw;
            }
        }

        public async Task<bool> ValidateIceCreamAndFlavourCombination(ICollection<FlavourInformation> flavours, IceCreamBase iceCreamBase)
        {
            try
            {
                if (iceCreamBase != IceCreamBase.Cup && flavours?.Count >= 4)
                {
                    return await Task.FromResult(false).ConfigureAwait(false);
                }

                return await Task.FromResult(true).ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> ValidateCookieDoughflavourtheSugarConeBase(FlavourInformation flavour, IceCreamBase iceCreamBase)
        {
            try
            {
                if (iceCreamBase == IceCreamBase.SugarCone && flavour?.IceCreamFlavour == IceCreamFlavour.CookieDough)
                {
                    return await Task.FromResult(false).ConfigureAwait(false);
                }

                return await Task.FromResult(true).ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> HandleErrorState(PurchaseInformation purchase)
        {
            switch (purchase?.State)
            {
                case PurchaseStates.AmountInvalid:
                    return await Task.FromResult($"Invalid purchaseAmount!. Please check the purchase Amount {purchase.IceCreamOrder.PurchaseAmount}").ConfigureAwait(false);
                case PurchaseStates.AmountLessThanCost:
                    return await Task.FromResult($"Invalid purchaseAmount!. Purchase Amount {purchase.IceCreamOrder.PurchaseAmount} must be greater than the cost amount {purchase.CostOfIceCream}.").ConfigureAwait(false);
                case PurchaseStates.TimeInvalid:
                    return await Task.FromResult($"Invalid purchase time. Please visit between store hours.").ConfigureAwait(false);
                case PurchaseStates.BaseInvalid:
                    return await Task.FromResult($"Invalid ice cream base. Please select a valid base.").ConfigureAwait(false);
                case PurchaseStates.FlavourInvalid:
                    return await Task.FromResult($"Invalid ice cream flavour. Please select a valid flavour.").ConfigureAwait(false);
                case PurchaseStates.NumberOfScoopsInvalid:
                    return await Task.FromResult($"Invalid number of scoops. Please select valid number of scoops").ConfigureAwait(false);
                case PurchaseStates.BaseFlavourCombinationInvalid:
                    return await Task.FromResult($"Invalid base and flavour combination. Please select a different combination").ConfigureAwait(false);
                case PurchaseStates.FlavourCombinationInvalid:
                    return await Task.FromResult($"Invalid flavour combination. Please select a different combination").ConfigureAwait(false);
                default:
                    return string.Empty;
            }
        }

        public async Task<string> GetUserName(HttpContext httpContext)
        {
            string authHeader = httpContext?.Request.Headers["Authorization"];
            string encodedUserNamePassword = authHeader.Substring("Basic".Length).Trim();
            Encoding encoding = Encoding.GetEncoding("UTF-8");
            string userNameAndPassword = encoding.GetString(Convert.FromBase64String(encodedUserNamePassword));
            int index = userNameAndPassword.IndexOf(":");
            var userName = userNameAndPassword.Substring(0, index);

            return await Task.FromResult(userName).ConfigureAwait(false);
        }
    }
}
