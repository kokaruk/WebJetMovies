﻿using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using WebJetMoviesAPI.Core;
using WebJetMoviesAPI.Data;
using WebJetMoviesAPI.Utils;
using WebJetMoviesAPI.Utils.SettingsModels;
using static WebJetMoviesAPI.Utils.PolicyHandler;

namespace WebJetMoviesAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILoggerFactory logFactory)
        {
            Configuration = configuration;
            StaticLogger.LoggerFactory = logFactory;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // configuration middleware options access
            services.Configure<CinemaServicesOptions>(Configuration.GetSection(nameof(CinemaServicesOptions)));
            services.Configure<PaginationOptions>(Configuration.GetSection(nameof(PaginationOptions)));
            services.Configure<PosterServiceOptions>(Configuration.GetSection(nameof(PosterServiceOptions)));

            // working behind proxy
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownProxies.Add(IPAddress.Parse("192.168.1.75"));
            });

            // Register the Swagger services
            services.AddSwaggerDocument(document => { document.PostProcess = d => { d.Info.Title = "WEBJET ASR"; }; });

            // response caching middleware
            services.AddResponseCaching();

            // memory cache
            services.AddMemoryCache();

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // add CORS support
            services.AddCors(options =>
            {
                options.AddPolicy("MaxOpen",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            services.AddHttpClient<IApiService, ApiService>(
                    c =>
                    {
                        c.DefaultRequestHeaders.Add("x-access-token", Configuration["x-access-token"]);
                        c.BaseAddress = new Uri(Configuration["WebApiUrl"]);
                    })
                .SetHandlerLifetime(TimeSpan.FromMinutes(30))
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy())
                .AddPolicyHandler(GetTimeOutPolicy());

            services.AddHttpClient<IPosterService, PosterService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(30))
                .AddPolicyHandler(GetRetryPolicy(3))
                .AddPolicyHandler(GetCircuitBreakerPolicy(3))
                .AddPolicyHandler(GetTimeOutPolicy(3));

            services.AddHealthChecks();
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // add swagger ui for development
                app.UseSwagger(config =>
                {
                    config.Path = "/swagger/v1/swagger.json";
                    config.PostProcess = (document, request) =>
                    {
                        if (request.Headers.ContainsKey("X-External-Host"))
                        {
                            // Change document server settings to public
                            document.Host = request.Headers["X-External-Host"].First();
                            document.BasePath = request.Headers["X-External-Path"].First();
                        }
                    };
                });
                app.UseSwaggerUi3();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                // The following StatusCodes are the default assignments for
                // the HealthStatus properties.
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK
                }
            });

            app.Use(async (context, next) =>
            {
                // response caching config
                context.Response.GetTypedHeaders().CacheControl =
                    new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromMinutes(1)
                    };
                context.Response.Headers[HeaderNames.Vary] =
                    new[] {"Accept-Encoding"};

                await next();
            });

            app.UseResponseCaching();

            app.UseCors("MaxOpen");


//          proxy should handle https
//          app.UseHttpsRedirection();

            app.UseMvc();
        }
    }
}
