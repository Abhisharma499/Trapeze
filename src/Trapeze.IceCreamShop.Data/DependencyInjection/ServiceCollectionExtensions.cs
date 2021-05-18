// <copyright file="ServiceCollectionExtensions.cs" company="Trapeze Ice Cream">
// Copyright (c) Trapeze Ice Cream. All rights reserved.
// </copyright>

namespace Trapeze.IceCreamShop.Data.DependencyInjection
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Trapeze.IceCreamShop.Data.DAL;

    /// <summary>
    /// Extension methods for the <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the data layer services necessary.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the data layer services too.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> containing the configuration items for the data layer.</param>
        /// <returns>The modified <see cref="IServiceCollection"/> containing the data layer services.</returns>
        public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IceCreamDbContext>(options => options.UseSqlite(configuration.GetConnectionString("IceCreamDb")));

            return services;
        }
    }
}
