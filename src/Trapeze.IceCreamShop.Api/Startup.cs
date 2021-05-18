// <copyright file="Startup.cs" company="Trapeze Ice Cream">
// Copyright (c) Trapeze Ice Cream. All rights reserved.
// </copyright>

namespace Trapeze.IceCreamShop.Api
{
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using CorrelationId;
    using CorrelationId.Abstractions;
    using CorrelationId.DependencyInjection;
    using Hellang.Middleware.ProblemDetails;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Trapeze.IceCreamShop.Api.Filters;
    using Trapeze.IceCreamShop.Data;
    using Trapeze.IceCreamShop.Data.DAL;

    /// <summary>
    /// An implementation to initialize an instance of the stateless service.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">A <see cref="IConfiguration"/> with the configuration infomation.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration object.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">A <see cref="IServiceCollection"/> to add the services to.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Adds top level correlation id creation
            services.AddDefaultCorrelationId(options =>
            {
                options.AddToLoggingScope = true;
                options.EnforceHeader = false;
                options.IgnoreRequestHeader = false;
                options.IncludeInResponse = true;
                options.RequestHeader = "X-Correlation-Id";
                options.ResponseHeader = "X-Correlation-Id";
                options.UpdateTraceIdentifier = false;
            });

            // Adds top level exception handling
            services.AddProblemDetails(options =>
            {
                options.GetTraceId = (ctx) => ctx.RequestServices.GetRequiredService<ICorrelationContextAccessor>()?.CorrelationContext?.CorrelationId ?? ctx.TraceIdentifier;
                options.IncludeExceptionDetails = (ctx, e) => ctx.RequestServices.GetRequiredService<IWebHostEnvironment>()?.IsDevelopment() ?? false;
                options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);
            });

            services.AddTransient<IiceCreamService, IiceCreamService>();

            services.AddDbContext<IceCreamDbContext>(options => options.UseSqlite("Data Source=icecream.db"));

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                    options.JsonSerializerOptions.WriteIndented = true;
                });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">A <see cref="IApplicationBuilder"/> to modify.</param>
        /// <param name="env">A <see cref="IWebHostEnvironment"/> with the environment of the api.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Adds top level exception handling middleware and correlation id middleware
            app.UseCorrelationId()
                .UseProblemDetails();

            app.UseMiddleware<UserAuthenticationMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
