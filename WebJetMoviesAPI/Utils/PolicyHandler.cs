using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace WebJetMoviesAPI.Utils
{
    /// <summary>
    ///     Handling policies for Polly / HttpClient 
    /// </summary>
    public static class PolicyHandler
    {
        private static readonly ILogger _logger = StaticLogger.CreateLogger("PolicyHandler");

        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int seconds = 1)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(11,
                    retryAttempt =>
                    {
                        _logger.LogWarning($"Retry count {retryAttempt}");
                        return TimeSpan.FromSeconds(seconds);
                    });
        }

        public static IAsyncPolicy<HttpResponseMessage> GetTimeOutPolicy(int seconds = 1)
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(seconds));
        }

        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int seconds = 1)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(seconds));
        }
    }
}