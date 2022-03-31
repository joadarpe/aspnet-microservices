using System;
using System.Security.Claims;
using Common.Auth;
using Common.Logging;
using Common.Resilience;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog;
using Shopping.WebApp.Services;

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
builder.AddTransientLoggingDelegatingHandler();

// Add services to the container.
builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
    c.BaseAddress = new Uri(builder.Configuration.GetConnectionString("GatewayAddress"))
)
.AddAuthenticationDelegatingHandler()
.AddLoggingDelegatingHandler()
.AddWaitAndRetryAsyncPolicyHandler(builder.Configuration)
.AddCircuitBreakerAsyncPolicyHandler(builder.Configuration);

builder.Services.AddHttpClient<IBasketService, BasketService>(c =>
    c.BaseAddress = new Uri(builder.Configuration.GetConnectionString("GatewayAddress"))
)
.AddAuthenticationDelegatingHandler()
.AddLoggingDelegatingHandler()
.AddWaitAndRetryAsyncPolicyHandler(builder.Configuration)
.AddCircuitBreakerAsyncPolicyHandler(builder.Configuration);

builder.Services.AddHttpClient<IOrderService, OrderService>(c =>
    c.BaseAddress = new Uri(builder.Configuration.GetConnectionString("GatewayAddress"))
)
.AddAuthenticationDelegatingHandler()
.AddLoggingDelegatingHandler()
.AddWaitAndRetryAsyncPolicyHandler(builder.Configuration)
.AddCircuitBreakerAsyncPolicyHandler(builder.Configuration);

builder.Services.AddRazorPages();

builder.Host.UseSerilog();

builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri(builder.Configuration.GetConnectionString("GatewayAddress")), "Ocelot.ApiGw", HealthStatus.Degraded);

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
    endpoints.MapHealthChecks("/health", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
});

app.Run();