using System;
using Common.Logging;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Shopping.Aggregator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
    c.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiSettings:CatalogUrl")));
builder.Services.AddHttpClient<IBasketService, BasketService>(c =>
    c.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiSettings:BasketUrl")));
builder.Services.AddHttpClient<IOrderService, OrderService>(c =>
    c.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiSettings:OrderingUrl")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog();

builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri($"{builder.Configuration.GetValue<string>("ApiSettings:CatalogUrl")}/swagger/index.html"), "Catalog.API", HealthStatus.Degraded)
    .AddUrlGroup(new Uri($"{builder.Configuration.GetValue<string>("ApiSettings:BasketUrl")}/swagger/index.html"), "Basket.API", HealthStatus.Degraded)
    .AddUrlGroup(new Uri($"{builder.Configuration.GetValue<string>("ApiSettings:OrderingUrl")}/swagger/index.html"), "Ordering.API", HealthStatus.Degraded);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

