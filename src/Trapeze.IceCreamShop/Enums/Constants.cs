namespace Trapeze.IceCreamShop.Enums
{
    using System;
    using System.Collections.Generic;

    public static class Constants
    {
        public const int PriceWaffleCone = 4;

        public const int PriceNonWaffleCone = 3;

        public const int MaxFlavourCountAllowed = 4;

        public const int MinFlavourCountAllowed = 1;

        public static IReadOnlyCollection<DayOfWeek> AllowedDaysForPurchase = new List<DayOfWeek>() { DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday };

        public static TimeSpan StartTimeOfStore = new TimeSpan(9, 0, 0); //10 o'clock)
        public static TimeSpan EndTimeOfStore = new TimeSpan(23, 45, 0); //5:45PM

        public static List<IceCreamFlavour> FlavoursRestrictedTogether = new List<IceCreamFlavour>() { IceCreamFlavour.Strawberry, IceCreamFlavour.MintChocolateChip };

        public static List<IceCreamFlavour> FlavoursRestrictedTogether2 = new List<IceCreamFlavour>() { IceCreamFlavour.CookiesAndCream, IceCreamFlavour.MooseTracks, IceCreamFlavour.Vanilla };


    }
}
