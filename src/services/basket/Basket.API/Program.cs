using System;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Common.Logging;
using Discount.gRPC.Protos;
using HealthChecks.UI.Client;
using IdentityClient.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Redis config
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("BasketDB");
});
// Services
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
// Grpc config
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration.GetConnectionString("GrpcDiscountUrl"));
});
builder.Services.AddScoped<DiscountGrpcService>();
// RabbitMQ config
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("EventBusAddress"));
    });
});
builder.Services.AddMassTransitHostedService();
// Automapper
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddApiAuthentication();

builder.Host.UseSerilog();

builder.Services.AddHealthChecks()
    .AddRedis(redisConnectionString: builder.Configuration.GetConnectionString("BasketDB"))
    .AddRabbitMQ(rabbitConnectionString: builder.Configuration.GetConnectionString("EventBusAddress"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
