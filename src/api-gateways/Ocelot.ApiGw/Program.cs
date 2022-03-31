using Common.Auth;
using Common.Logging;
using Common.Tracing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddApiGatewayAuthentication();

builder.Configuration.AddJsonFile($"ocelot.json", true, true);
builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true);
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddOcelot().AddCacheManager(cb =>
{
    cb.WithDictionaryHandle();
});

builder.Host.UseSerilog();

builder.AddZipkinTelemetryTracing();

var app = builder.Build();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", () => "Hello World!");
});

await app.UseOcelot();

app.Run();

