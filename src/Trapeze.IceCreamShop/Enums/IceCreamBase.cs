// <copyright file="IceCreamBase.cs" company="Trapeze Ice Cream">
// Copyright (c) Trapeze Ice Cream. All rights reserved.
// </copyright>

namespace Trapeze.IceCreamShop.Enums
{
    /// <summary>
    /// The types of ice cream bases that can be purchased.
    /// </summary>
    public enum IceCreamBase
    {
        /// <summary>
        /// The ice cream is in a paper/plastic cup.
        /// </summary>
        Cup,

        /// <summary>
        /// The ice cream is in a cake cone (see https://www.webstaurantstore.com/109/ice-cream-cones.html?filter=cone-type:cake).
        /// </summary>
        CakeCone,

        /// <summary>
        /// The ice cream is in a sugar cone (see https://www.webstaurantstore.com/109/ice-cream-cones.html?filter=cone-type:sugar).
        /// </summary>
        SugarCone,

        /// <summary>
        /// The ice cream is in a waffle cone (see https://www.webstaurantstore.com/109/ice-cream-cones.html?filter=cone-type:waffle).
        /// </summary>
        WaffleCone
    }
}
