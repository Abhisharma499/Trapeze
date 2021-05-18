// <copyright file="BaseInformationConfig.cs" company="Trapeze Ice Cream">
// Copyright (c) Trapeze Group. All rights reserved.
// </copyright>

namespace Trapeze.IceCreamShop.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Trapeze.IceCreamShop.Data.Entities;

    /// <summary>
    /// A configuration class for the BaseInformation entity.
    /// </summary>
    internal sealed class BaseInformationConfig : IEntityTypeConfiguration<BaseInformation>
    {
        /// <summary>
        /// Configures the BaseInformation entity.
        /// </summary>
        /// <param name="builder">A <see cref="EntityTypeBuilder{BaseInformation}"/> used to build up the EF entity information.</param>
        public void Configure(EntityTypeBuilder<BaseInformation> builder)
        {
            builder.HasOne(x => x.IceCreamInformation)
                .WithOne(x => x.Base)
                .HasForeignKey<BaseInformation>(x => x.ParentId);

            builder.HasKey(x => x.Id)
                .HasName("pkBase");

            builder.HasIndex(x => x.ParentId)
                .IsUnique();

            builder.ToTable(nameof(BaseInformation));
        }
    }
}
