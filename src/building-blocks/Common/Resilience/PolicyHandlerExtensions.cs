using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace Common.Resilience
{
    public static class PolicyHandlerExtensions
    {

        /// <summary>
        /// Adds WaitAndRetryAsync to <c>IHttpClientBuilder</c>
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns><c>IHttpClientBuilder</c></returns>
        public static IHttpClientBuilder AddWaitAndRetryAsyncPolicyHandler(this IHttpClientBuilder clientBuilder, IConfiguration configuration = null)
        {
            var config = configuration?.GetSection("WaitAndRetry");
            var maxAttemps = config?.GetValue<int?>("RetryCount") ?? 5;
            var exponentialMultiplier = config?.GetValue<int?>("ExponentialBackoffMultiplier") ?? 2;

            return clientBuilder.AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: maxAttemps,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(exponentialMultiplier, retryAttempt)),
                    onRetry: (exception, retryCount, context) =>
                    {
                        Log.Error($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                    }));
        }

        /// <summary>
        /// Adds CircuitBreakerAsync to <c>IHttpClientBuilder</c>
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns><c>IHttpClientBuilder</c></returns>
        public static IHttpClientBuilder AddCircuitBreakerAsyncPolicyHandler(this IHttpClientBuilder clientBuilder, IConfiguration configuration = null)
        {
            var config = configuration?.GetSection("CircuitBreaker");
            var eventsAllowedBeforeBreaking = config?.GetValue<int?>("EventsBeforeBreaking") ?? 5;
            var durationOfBreakInSeconds = config?.GetValue<int?>("DurationOfBreak") ?? 30;

            return clientBuilder.AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: eventsAllowedBeforeBreaking,
                    durationOfBreak: TimeSpan.FromSeconds(durationOfBreakInSeconds)
                ));
        }
    }
}

