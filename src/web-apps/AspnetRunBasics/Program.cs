using System;
using System.Security.Claims;
using AspnetRunBasics.HttpHandlers;
using AspnetRunBasics.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
});

// Add services to the container.
builder.Services.AddTransient<AuthenticationDelegatingHandler>();
builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
    c.BaseAddress = new Uri(builder.Configuration.GetConnectionString("GatewayAddress"))
).AddHttpMessageHandler<AuthenticationDelegatingHandler>();
builder.Services.AddHttpClient<IBasketService, BasketService>(c =>
    c.BaseAddress = new Uri(builder.Configuration.GetConnectionString("GatewayAddress"))
).AddHttpMessageHandler<AuthenticationDelegatingHandler>();
builder.Services.AddHttpClient<IOrderService, OrderService>(c =>
    c.BaseAddress = new Uri(builder.Configuration.GetConnectionString("GatewayAddress"))
).AddHttpMessageHandler<AuthenticationDelegatingHandler>();
// Identity server service
builder.Services.AddSingleton(new ClientCredentialsTokenRequest()
{
    Address = "connect/token",
    ClientId = "api.client",
    ClientSecret = "api.secret",
    Scope = "api.scope"
});
builder.Services.AddHttpClient<IIdentityService, IdentityService>(c =>
    c.BaseAddress = new Uri(builder.Configuration.GetConnectionString("IdentityAuthority")));

builder.Services.AddRazorPages();

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