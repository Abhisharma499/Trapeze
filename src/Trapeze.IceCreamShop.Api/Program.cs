// <copyright file="Program.cs" company="Trapeze Ice Cream">
// Copyright (c) Trapeze Ice Cream. All rights reserved.
// </copyright>

namespace Trapeze.IceCreamShop.Api
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Entry point and setup process for the Ice Cream API.
    /// </summary>
    public sealed class Program
    {
        /// <summary>
        /// The entry method for the Ice Cream API.
        /// </summary>
        /// <param name="args">A <see cref="string"/>[] with the arguments for the API.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates the host.
        /// </summary>
        /// <param name="args">A <see cref="string"/>[] with the arguments for the API.</param>
        /// <returns>An <see cref="IHostBuilder"/>.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
