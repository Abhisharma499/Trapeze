// <copyright file="IceCreamDbContextShould.cs" company="Trapeze Ice Cream">
// Copyright (c) Trapeze Group. All rights reserved.
// </copyright>

namespace Trapeze.PASS.Address.UnitTests
{
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Trapeze.IceCreamShop.Data;
    using Trapeze.IceCreamShop.Data.DependencyInjection;
    using Trapeze.IceCreamShop.Data.Entities;
    using Trapeze.IceCreamShop.Enums;
    using Xunit;

    /// <summary>
    /// Test cases for the ice cream db-context services.
    /// </summary>
    public class IceCreamDbContextShould
    {
        /// <summary>
        /// Tests the ability to purchase a single ice cream.
        /// </summary>
        /// <returns>A <see cref="Task"/>.</returns>
        [Fact]
        public async Task PurchaseIceCream()
        {
            var provider = BuildServiceProvider();
            var sut = provider.GetRequiredService<IceCreamDbContext>();

            var info = new IceCreamInformation()
            {
                PurchaseAmount = 5.00M,
                PurchaserName = "Bob",
                Base = new BaseInformation()
                {
                    IceCreamBase = IceCreamBase.Cup
                }
            };

            info.Flavours.Add(new FlavourInformation()
            {
                IceCreamFlavour = IceCreamFlavour.Chocolate
            });

            // Add the new ice cream purchase
            await sut.AddAsync<IceCreamInformation>(info).ConfigureAwait(false);
            await sut.SaveChangesAsync().ConfigureAwait(false);

            // Check the added ice cream
            var oldInfo = await sut.IceCreams.FindAsync(new object[] { 1 }).ConfigureAwait(false);

            oldInfo.Id.Should().Be(1);
            oldInfo.PurchaseAmount.Should().Be(5.00M);
            oldInfo.PurchaserName.Should().Be("Bob");
            oldInfo.PurchaseDateTime.Should().BeBefore(DateTime.UtcNow);
            oldInfo.PurchaseDateTime.Should().BeAfter(DateTime.UtcNow.AddMinutes(-2));

            oldInfo.Base.Should().NotBeNull();
            oldInfo.Base.ParentId.Should().Be(1);
            oldInfo.Base.IceCreamBase.Should().Be(IceCreamBase.Cup);

            oldInfo.Flavours.Count.Should().Be(1);

            foreach (var flavour in oldInfo.Flavours)
            {
                flavour.Id.Should().Be(1);
                flavour.ParentId.Should().Be(1);
                flavour.IceCreamFlavour.Should().Be(IceCreamFlavour.Chocolate);
            }
        }

        /// <summary>
        /// Tests the ability to purchase multiple ice creams at one time.
        /// </summary>
        /// <returns>A <see cref="Task"/>.</returns>
        [Fact]
        public async Task PurchaseMultipleIceCreams()
        {
            var provider = BuildServiceProvider();
            var sut = provider.GetRequiredService<IceCreamDbContext>();

            var info1 = new IceCreamInformation()
            {
                PurchaseAmount = 8.00M,
                PurchaserName = "Tom",
                Base = new BaseInformation()
                {
                    IceCreamBase = IceCreamBase.WaffleCone
                }
            };

            info1.Flavours.Add(new FlavourInformation()
            {
                IceCreamFlavour = IceCreamFlavour.Chocolate
            });

            info1.Flavours.Add(new FlavourInformation()
            {
                IceCreamFlavour = IceCreamFlavour.Vanilla
            });

            info1.Flavours.Add(new FlavourInformation()
            {
                IceCreamFlavour = IceCreamFlavour.MintChocolateChip
            });

            // Add the first ice cream purchase
            await sut.AddAsync<IceCreamInformation>(info1).ConfigureAwait(false);

            var info2 = new IceCreamInformation()
            {
                PurchaseAmount = 7.00M,
                PurchaserName = "Steve",
                Base = new BaseInformation()
                {
                    IceCreamBase = IceCreamBase.SugarCone
                }
            };

            info2.Flavours.Add(new FlavourInformation()
            {
                IceCreamFlavour = IceCreamFlavour.CookiesAndCream
            });

            info2.Flavours.Add(new FlavourInformation()
            {
                IceCreamFlavour = IceCreamFlavour.Strawberry
            });

            // Add the second ice cream purchase
            await sut.AddAsync<IceCreamInformation>(info2).ConfigureAwait(false);
            await sut.SaveChangesAsync().ConfigureAwait(false);

            // Check the first added ice cream
            var oldInfo1 = await sut.IceCreams.FindAsync(new object[] { 1 }).ConfigureAwait(false);

            oldInfo1.Id.Should().Be(1);
            oldInfo1.PurchaseAmount.Should().Be(8.00M);
            oldInfo1.PurchaserName.Should().Be("Tom");
            oldInfo1.PurchaseDateTime.Should().BeBefore(DateTime.UtcNow);
            oldInfo1.PurchaseDateTime.Should().BeAfter(DateTime.UtcNow.AddMinutes(-2));

            oldInfo1.Base.Should().NotBeNull();
            oldInfo1.Base.ParentId.Should().Be(1);
            oldInfo1.Base.IceCreamBase.Should().Be(IceCreamBase.WaffleCone);

            oldInfo1.Flavours.Count.Should().Be(3);

            var loop = 0;
            foreach (var flavour in oldInfo1.Flavours)
            {
                var currentFlavour = loop switch
                {
                    0 => IceCreamFlavour.Chocolate,
                    1 => IceCreamFlavour.Vanilla,
                    2 => IceCreamFlavour.MintChocolateChip,
                    _ => IceCreamFlavour.MooseTracks,
                };

                flavour.Id.Should().Be(loop + 1);
                flavour.ParentId.Should().Be(1);
                flavour.IceCreamFlavour.Should().Be(currentFlavour);

                loop++;
            }

            // Check the second added ice cream
            var oldInfo2 = await sut.IceCreams.FindAsync(new object[] { 2 }).ConfigureAwait(false);

            oldInfo2.Id.Should().Be(2);
            oldInfo2.PurchaseAmount.Should().Be(7.00M);
            oldInfo2.PurchaserName.Should().Be("Steve");
            oldInfo2.PurchaseDateTime.Should().BeBefore(DateTime.UtcNow);
            oldInfo2.PurchaseDateTime.Should().BeAfter(DateTime.UtcNow.AddMinutes(-2));

            oldInfo2.Base.Should().NotBeNull();
            oldInfo2.Base.ParentId.Should().Be(2);
            oldInfo2.Base.IceCreamBase.Should().Be(IceCreamBase.SugarCone);

            oldInfo2.Flavours.Count.Should().Be(2);

            loop = 0;
            foreach (var flavour in oldInfo2.Flavours)
            {
                var currentFlavour = loop switch
                {
                    0 => IceCreamFlavour.CookiesAndCream,
                    1 => IceCreamFlavour.Strawberry,
                    _ => IceCreamFlavour.MooseTracks,
                };

                flavour.Id.Should().Be(loop + 4);
                flavour.ParentId.Should().Be(2);
                flavour.IceCreamFlavour.Should().Be(currentFlavour);

                loop++;
            }
        }

        /// <summary>
        /// Tests the ability to purchase a single ice cream with multiple scoops of the same flavour.
        /// </summary>
        /// <returns>A <see cref="Task"/>.</returns>
        [Fact]
        public async Task PurchaseSameFlavourIceCream()
        {
            var provider = BuildServiceProvider();
            var sut = provider.GetRequiredService<IceCreamDbContext>();

            var info = new IceCreamInformation()
            {
                PurchaseAmount = 7.00M,
                PurchaserName = "John",
                Base = new BaseInformation()
                {
                    IceCreamBase = IceCreamBase.CakeCone
                }
            };

            info.Flavours.Add(new FlavourInformation()
            {
                IceCreamFlavour = IceCreamFlavour.MooseTracks
            });

            info.Flavours.Add(new FlavourInformation()
            {
                IceCreamFlavour = IceCreamFlavour.MooseTracks
            });

            info.Flavours.Add(new FlavourInformation()
            {
                IceCreamFlavour = IceCreamFlavour.MooseTracks
            });

            // Add the new ice cream purchase
            await sut.AddAsync<IceCreamInformation>(info).ConfigureAwait(false);
            await sut.SaveChangesAsync().ConfigureAwait(false);

            // Check the added ice cream
            var oldInfo = await sut.IceCreams.FindAsync(new object[] { 1 }).ConfigureAwait(false);

            oldInfo.Id.Should().Be(1);
            oldInfo.PurchaseAmount.Should().Be(7.00M);
            oldInfo.PurchaserName.Should().Be("John");
            oldInfo.PurchaseDateTime.Should().BeBefore(DateTime.UtcNow);
            oldInfo.PurchaseDateTime.Should().BeAfter(DateTime.UtcNow.AddMinutes(-2));

            oldInfo.Base.Should().NotBeNull();
            oldInfo.Base.ParentId.Should().Be(1);
            oldInfo.Base.IceCreamBase.Should().Be(IceCreamBase.CakeCone);

            oldInfo.Flavours.Count.Should().Be(3);

            foreach (var flavour in oldInfo.Flavours)
            {
                flavour.Id.Should().BeInRange(1, 3);
                flavour.ParentId.Should().Be(1);
                flavour.IceCreamFlavour.Should().Be(IceCreamFlavour.MooseTracks);
            }
        }

        /// <summary>
        /// Builds the IOC container.
        /// </summary>
        /// <returns>A <see cref="IServiceProvider"/>.</returns>
        private static IServiceProvider BuildServiceProvider()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName)
               .AddJsonFile("TestData\\appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddTransient<IConfiguration>(src => configuration);
            services.AddDataLayer(configuration);
            return services.BuildServiceProvider();
        }
    }
}