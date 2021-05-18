// <copyright file="FlavourInformation.cs" company="Trapeze Ice Cream">
// Copyright (c) Trapeze Ice Cream. All rights reserved.
// </copyright>

namespace Trapeze.IceCreamShop.Data.Entities
{
    using System.ComponentModel.DataAnnotations;
    using Trapeze.IceCreamShop.Enums;

    /// <summary>
    /// The scoop flavours an ice cream has on it.
    /// </summary>
    public class FlavourInformation
    {
        /// <summary>
        /// Gets or sets the id of the flavour information.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the id of the parent information.
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// Gets or sets the flavour of the ice cream.
        /// </summary>
        public IceCreamFlavour IceCreamFlavour { get; set; }

        /// <summary>
        /// Gets or sets the parent ice cream information.
        /// </summary>
        public virtual IceCreamInformation IceCreamInformation { get; set; }
    }
}
