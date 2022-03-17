using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Configuration;

namespace IdentityServer
{
    public class Config
    {
        public static IEnumerable<Client> BuildClients(IConfiguration config)
        {
            var webAppClientURL = config.GetConnectionString("WebAppClient")
                ?? throw new ArgumentNullException("Required ConnectionStrings:WebAppClient");

            return new Client[]
            {
                new Client
                {
                    ClientId = "shopping.webapp.client",
                    ClientName = "Shopping webapp",
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowRememberConsent = false,
                    RedirectUris =
                    {
                        $"{webAppClientURL}/signin-oidc",
                    },
                    PostLogoutRedirectUris =
                    {
                        $"{webAppClientURL}/signout-callback-oidc",
                    },
                    ClientSecrets =
                    {
                        new Secret("shopping.webapp.secret".Sha256())
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },
                new Client
                {
                    ClientId = "api.client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("api.secret".Sha256())
                    },
                    AllowedScopes =
                    {
                        "api.scope"
                    }
                }
            };
        }

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("api.scope")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static List<TestUser> TestUsers =>
            new()
            {
                new TestUser
                {
                    SubjectId = "JonathanA",
                    Username = "JonathanA",
                    Password = "joadarpe",
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.GivenName, "Jonathan"),
                        new Claim(JwtClaimTypes.FamilyName, "A")
                    }
                },
                new TestUser
                {
                    SubjectId = "LuisaS",
                    Username = "LuisaS",
                    Password = "luigifer",
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.GivenName, "Luisa"),
                        new Claim(JwtClaimTypes.FamilyName, "S")
                    }
                }
            };
    }
}