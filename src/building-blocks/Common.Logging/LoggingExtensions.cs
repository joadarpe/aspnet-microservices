using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Common.Logging
{
    public static class LoggingExtensions
	{
        /// <summary>
        /// Configures Serilog with ElasticSearch
        /// <para>
        /// Requires a ConnectionStrings:ElasticSearchUri environment value
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static void UseSerilog(this IHostBuilder builder)
        {
            builder.UseSerilog((context, config) =>
            {
                var elasticUri = context.Configuration.GetConnectionString("ElasticSearchUri")
                    ?? throw new ArgumentNullException("Required a valid ConnectionStrings:ElasticSearchUri");

                config.Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .WriteTo.Console()
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                    {
                        IndexFormat = $"applogs-{context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                        AutoRegisterTemplate = true,
                        NumberOfShards = 2,
                        NumberOfReplicas = 1
                    })
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithEnvironmentUserName()
                    .ReadFrom.Configuration(context.Configuration);
            });
        }
    }
}

