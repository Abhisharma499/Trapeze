// <copyright file="IceCreamInformation.cs" company="Trapeze Ice Cream">
// Copyright (c) Trapeze Ice Cream. All rights reserved.
// </copyright>

namespace Trapeze.IceCreamShop.Data.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The information associated with the purchase of an ice cream.
    /// </summary>
    public class IceCreamInformation
    {
        /// <summary>
        /// Gets or sets the id of the purchase transaction.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the purchase amount.
        /// </summary>
        public decimal PurchaseAmount { get; set; }

        /// <summary>
        /// Gets or sets the name of the purchaser.
        /// </summary>
        public string PurchaserName { get; set; }

        /// <summary>
        /// Gets the date and time of the purchase.
        /// </summary>
        public DateTime PurchaseDateTime { get; set; }

        /// <summary>
        /// Gets or sets the base of the ice cream.
        /// </summary>
        public BaseInformation Base { get; set; }

        /// <summary>
        /// Gets the flavours of ice cream.
        /// </summary>
        public ICollection<FlavourInformation> Flavours { get; set; } = new List<FlavourInformation>();
    }
}
