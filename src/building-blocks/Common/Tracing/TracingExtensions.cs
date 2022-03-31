using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

namespace Common.Tracing
{
    public static class TracingExtensions
	{
        /// <summary>
        /// Adds Open Telemetry Tracing with Zipkin Exporter
        /// <para>
        /// Requires a ConnectionStrings:ZipkinUri environment value
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
		public static void AddZipkinTelemetryTracing(this WebApplicationBuilder builder)
        {
            var zipkinUri = builder.Configuration.GetConnectionString("ZipkinUri")
                       ?? throw new ArgumentNullException("Required a valid ConnectionStrings:ZipkinUri");

            builder.Services.AddOpenTelemetryTracing(config =>
            {
                config.AddZipkinExporter(config =>
                {
                    config.Endpoint = new Uri($"{zipkinUri}/api/v2/spans");
                })
                .AddSource(builder.Environment.ApplicationName)
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation();
            });
        }
	}
}

