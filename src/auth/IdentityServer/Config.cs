using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer
{
    public class Config
    {
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "shopping.webapp.client",
                    ClientName = "Shopping webapp",
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowRememberConsent = false,
                    RedirectUris =
                    {
                        "https://localhost:5070/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:5070/signout-callback-oidc"
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
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
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