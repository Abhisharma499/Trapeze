// <copyright file="IceCreamInformationConfig.cs" company="Trapeze Ice Cream">
// Copyright (c) Trapeze Group. All rights reserved.
// </copyright>

namespace Trapeze.IceCreamShop.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Trapeze.IceCreamShop.Data.Entities;

    /// <summary>
    /// A configuration class for the IceCreamInformation entity.
    /// </summary>
    internal sealed class IceCreamInformationConfig : IEntityTypeConfiguration<IceCreamInformation>
    {
        /// <summary>
        /// Configures the IceCreamInformation entity.
        /// </summary>
        /// <param name="builder">A <see cref="EntityTypeBuilder{IceCreamInformation}"/> used to build up the EF entity information.</param>
        public void Configure(EntityTypeBuilder<IceCreamInformation> builder)
        {
            builder.HasOne(x => x.Base)
                .WithOne(x => x.IceCreamInformation)
                .HasForeignKey<IceCreamInformation>(x => x.Id);

            builder.HasMany(x => x.Flavours)
                .WithOne(x => x.IceCreamInformation)
                .HasForeignKey(x => x.ParentId);

            builder.HasKey(x => x.Id)
                .HasName("pkIceCream");

            builder.Property(x => x.PurchaseDateTime)
                .HasDefaultValueSql("DATETIME('now')");

            builder.ToTable(nameof(IceCreamInformation));
        }
    }
}
