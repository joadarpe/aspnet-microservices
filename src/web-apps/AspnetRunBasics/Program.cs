using System;
using System.Reflection;
using System.Security.Claims;
using AspnetRunBasics.Services;
using IdentityClient.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = builder.Configuration.GetConnectionString("IdentityAuthority");

    options.ClientId = "shopping.webapp.client";
    options.ClientSecret = "shopping.webapp.secret";
    options.ResponseType = "code";
    options.TokenValidationParameters.NameClaimType = ClaimTypes.NameIdentifier;

    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.SaveTokens = true;

    options.GetClaimsFromUserInfoEndpoint = true;
    options.RequireHttpsMetadata = false;
});

builder.AddApiClientAuthentication();

// Add services to the container.
builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
    c.BaseAddress = new Uri(builder.Configuration.GetConnectionString("GatewayAddress"))
).AddAuthenticationDelegatingHandler();
builder.Services.AddHttpClient<IBasketService, BasketService>(c =>
    c.BaseAddress = new Uri(builder.Configuration.GetConnectionString("GatewayAddress"))
).AddAuthenticationDelegatingHandler();
builder.Services.AddHttpClient<IOrderService, OrderService>(c =>
    c.BaseAddress = new Uri(builder.Configuration.GetConnectionString("GatewayAddress"))
).AddAuthenticationDelegatingHandler();


builder.Services.AddRazorPages();
builder.Host.UseSerilog((context, config) =>
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});

app.Run();