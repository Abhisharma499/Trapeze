// <copyright file="IceCreamDbContext.cs" company="Trapeze Ice Cream">
// Copyright (c) Trapeze Group. All rights reserved.
// </copyright>

namespace Trapeze.IceCreamShop.Data
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Trapeze.IceCreamShop.Data.Configurations;
    using Trapeze.IceCreamShop.Data.Entities;

    /// <summary>
    /// Creates a database context for the ice cream entities.
    /// </summary>
    public partial class IceCreamDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IceCreamDbContext"/> class.
        /// </summary>
        /// <param name="options">The database context options.</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/>.</param>
        public IceCreamDbContext(
            DbContextOptions<IceCreamDbContext> options)
            : base(options)
        {
            Database.OpenConnection();
            Database.EnsureCreated();
        }

        public DbSet<IceCreamInformation> IceCreams { get; set; }

        public DbSet<BaseInformation> Bases { get; set; }

        public DbSet<FlavourInformation> Flavours { get; set; }

        public DbSet<UserInformation> Users { get; set; }

        /// <summary>
        /// Defines the model that entity framework will retrieve.
        /// </summary>
        /// <param name="modelBuilder">The builder to create the model with.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.ApplyConfiguration(new IceCreamInformationConfig());
            modelBuilder.ApplyConfiguration(new BaseInformationConfig());
            modelBuilder.ApplyConfiguration(new FlavourInformationConfig());
            base.OnModelCreating(modelBuilder);
        }
    }
}
