using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trapeze.IceCreamShop.Enums
{
   public enum PurchaseStates
    {
        PurchaseSucess,
        PurchaseAmountInvalid,
        PurchaseAmountLessThanCost,
        PurchaseTimeInvalid,
        PurchaseBaseInvalid,
        PurchaseNumberOfScoopsInvalid,
        PurchaseBaseFlavourCombinationInvalid,
        PurchaseFlavourCombinationInvalid,
        PurchaseDataBaseInsertionError

    }
}
