// <copyright file="FlavourInformationConfig.cs" company="Trapeze Ice Cream">
// Copyright (c) Trapeze Group. All rights reserved.
// </copyright>

namespace Trapeze.IceCreamShop.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Trapeze.IceCreamShop.Data.Entities;

    /// <summary>
    /// A configuration class for the FlavourInformation entity.
    /// </summary>
    internal sealed class FlavourInformationConfig : IEntityTypeConfiguration<FlavourInformation>
    {
        /// <summary>
        /// Configures the FlavourInformation entity.
        /// </summary>
        /// <param name="builder">A <see cref="EntityTypeBuilder{FlavourInformation}"/> used to build up the EF entity information.</param>
        public void Configure(EntityTypeBuilder<FlavourInformation> builder)
        {
            builder.HasOne(x => x.IceCreamInformation)
                .WithMany(x => x.Flavours)
                .HasForeignKey(x => x.ParentId);

            builder.HasKey(x => x.Id)
                .HasName("pkFlavour");

            builder.ToTable(nameof(FlavourInformation));
        }
    }
}
