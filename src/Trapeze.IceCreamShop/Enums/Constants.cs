using System;
using System.Collections.Generic;

namespace Trapeze.IceCreamShop.Enums
{
    public static class Constants
    {
        public const int PriceWaffleCone = 4;
        public const int PriceNonWaffleCone = 3;

        public const int MaxFlavourCountAllowed = 4;

        public const int MinFlavourCountAllowed = 4;

        public static IReadOnlyCollection<DayOfWeek> AllowedDaysForPurchase = new List<DayOfWeek>() { DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday };

        public static TimeSpan StartTimeOfStore = new TimeSpan(9, 0, 0); //10 o'clock)
        public static TimeSpan EndTimeOfStore = new TimeSpan(23, 45, 0); //5:45PM

        
    }
}
