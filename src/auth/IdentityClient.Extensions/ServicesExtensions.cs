using System;
using IdentityModel.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IdentityClient.Extensions
{
    public static class ServicesExtensions
	{
        private const string BEARER_AUTHORITY = "Bearer";
        private const string IDENTITY_API_KEY_AUTHORITY = "IdentityApiKey";

        private const string IDENTITY_AUTHORITY_CONFIG = "IdentityAuthority";

        private const string NO_IDENTITY_AUTHORITY_CONFIG_MESSAGE = $"Required a valid ConnectionStrings:{IDENTITY_AUTHORITY_CONFIG}";

        /// <summary>
        /// Adds Authentication configuration with Bearer token to API projects
        /// <para>
        /// Requires a ConnectionStrings:IdentityAuthority environment value
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddApiAuthentication(this WebApplicationBuilder builder)
        {
            var identityAuthority = builder.Configuration.GetConnectionString(IDENTITY_AUTHORITY_CONFIG)
                ?? throw new ArgumentNullException(NO_IDENTITY_AUTHORITY_CONFIG_MESSAGE);

            builder.Services.AddAuthentication(BEARER_AUTHORITY)
                .AddJwtBearer(BEARER_AUTHORITY, options =>
                {
                    options.Authority = identityAuthority;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });
        }

        /// <summary>
        /// Adds Authentication configuration with Bearer token to API Gateways projects
        /// <para>
        /// Requires a ConnectionStrings:IdentityAuthority environment value
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddApiGatewayAuthentication(this WebApplicationBuilder builder)
        {
            var identityAuthority = builder.Configuration.GetConnectionString(IDENTITY_AUTHORITY_CONFIG)
                ?? throw new ArgumentNullException(NO_IDENTITY_AUTHORITY_CONFIG_MESSAGE);

            builder.Services.AddAuthentication().AddJwtBearer(IDENTITY_API_KEY_AUTHORITY, options =>
            {
                options.Authority = identityAuthority;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false
                };
            });
        }

        /// <summary>
        /// Adds Authentication configuration with Bearer token to API client projects.
        /// <para>
        /// This requires to additionally add <c>AuthenticationDelegatingHandler</c> to IHttpClientBuilder
        /// </para>
        /// <para>
        /// Requires a ConnectionStrings:IdentityAuthority environment value
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddApiClientAuthentication(this WebApplicationBuilder builder)
        {
            var identityAuthority = builder.Configuration.GetConnectionString(IDENTITY_AUTHORITY_CONFIG)
                ?? throw new ArgumentNullException(NO_IDENTITY_AUTHORITY_CONFIG_MESSAGE);

            builder.Services.AddTransient<AuthenticationDelegatingHandler>();

            builder.Services.AddSingleton(new ClientCredentialsTokenRequest()
            {
                Address = "connect/token",
                ClientId = "api.client",
                ClientSecret = "api.secret",
                Scope = "api.scope"
            });
            builder.Services.AddHttpClient<IIdentityService, IdentityService>(c =>
                c.BaseAddress = new Uri(builder.Configuration.GetConnectionString(IDENTITY_AUTHORITY_CONFIG)));
        }


        /// <summary>
        /// Adds AuthenticationDelegatingHandler to manage httpClient authentication header
        /// <para>
        /// Requires the previous call of <c>AddApiClientAuthentication</c> only once
        /// </para>
        /// </summary>
        /// <returns><c>IHttpClientBuilder</c></returns>
        public static IHttpClientBuilder AddAuthenticationDelegatingHandler(this IHttpClientBuilder clientBuilder)
        {
            return clientBuilder.AddHttpMessageHandler<AuthenticationDelegatingHandler>();
        }

    }
}

