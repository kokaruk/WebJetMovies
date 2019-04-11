using System;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using Polly.Extensions.Http;
using Polly.Timeout;  


namespace WebJetMoviesAPI.Utils
{
    public static class PolicyHandler
    {
        private static ILogger _logger = StaticLogger.CreateLogger("PolicyHandler");


        public static IAsyncPolicy<HttpResponseMessage> GetCachePolicy()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var memoryCacheProvider = new MemoryCacheProvider(memoryCache);
            var cachePolicy = Policy.CacheAsync<HttpResponseMessage>(memoryCacheProvider.AsyncFor<HttpResponseMessage>(), 
                TimeSpan.FromMinutes(5));
            
            return cachePolicy;
        }
            
            
        
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
            HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .Or<TimeoutRejectedException>()
                .WaitAndRetryForeverAsync(
                    retryAttempt =>
                    {
                        _logger.LogWarning($"Retry count {retryAttempt}");
                        return TimeSpan.FromSeconds(1);
                    });
        public static IAsyncPolicy<HttpResponseMessage> GetTimeOutPolicy(int seconds = 1) =>
            Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(seconds));

        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(1));
        }
    }
}