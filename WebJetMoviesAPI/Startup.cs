using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using Polly.Extensions.Http;
using Polly.Registry;
using Polly.Timeout;
using WebJetMoviesAPI.Core;
using WebJetMoviesAPI.Data;
using static WebJetMoviesAPI.Utils.PolicyHandler;

namespace WebJetMoviesAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILoggerFactory logFactory)
        {
            Configuration = configuration;
            Utils.StaticLogger.LoggerFactory = logFactory;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // caching
            services.AddMemoryCache();
            services.AddSingleton<IAsyncCacheProvider, MemoryCacheProvider>();

            services.AddSingleton<Polly.Registry.IReadOnlyPolicyRegistry<string>, Polly.Registry.PolicyRegistry>((serviceProvider) =>
            {
                PolicyRegistry registry = new PolicyRegistry();
                registry.Add("myCachePolicy", 
                    Policy.CacheAsync<HttpResponseMessage>(
                        serviceProvider
                            .GetRequiredService<IAsyncCacheProvider>()
                            .AsyncFor<HttpResponseMessage>(),
                        TimeSpan.FromMinutes(5)));
                return registry;
            });
            // working behind proxy
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = 
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownProxies.Add(IPAddress.Parse("192.168.1.75"));
            });
            
            services.AddResponseCaching();
            
            services.AddMvc().AddJsonOptions(
                options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            // add CORS support
            services.AddCors();
            
            services.AddHttpClient<IApiService, ApiService>(
                    c =>
                    {
                        c.DefaultRequestHeaders.Add("x-access-token",Configuration["x-access-token"]);
                        c.BaseAddress = new Uri(Configuration["WebApiUrl"]);
                    })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetCachePolicy())
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy())
                .AddPolicyHandler(GetTimeOutPolicy());
        }

        

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}