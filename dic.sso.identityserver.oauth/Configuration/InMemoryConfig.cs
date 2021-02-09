using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace dic.sso.identityserver.oauth.Configuration
{
    public static class InMemoryConfig
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

        public static IEnumerable<ApiResource> GetApiResources() => new List<ApiResource>
        {
            new ApiResource("companyApi", "CompanyEmployee API")
            {
                Scopes = { "companyApi" }
            }
        };

        public static IEnumerable<ApiScope> GetApiScopes() => new List<ApiScope>
        {
            new ApiScope("companyApi", "CompanyEmployee API")
        };
        public static List<TestUser> GetUsers() => new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "a9ea0f25-b964-409f-bcce-c923266249b4",
                Username = "Mick",
                Password = "123456",
                Claims = new List<Claim>
                {
                    new Claim("given_name", "Mick"),
                    new Claim("family_name", "Mining")
                }
            },
            new TestUser
            {
                SubjectId = "c95ddb8c-79ec-488a-a485-fe57a1462340",
                Username = "Jane",
                Password = "123456",
                Claims = new List<Claim>
                {
                    new Claim("given_name", "Jane"),
                    new Claim("family_name", "Downing")
                }
            }
        };


        public static IEnumerable<Client> GetClients() => new List<Client>
        {
            new Client
            {
                ClientId = "company-employee",
                ClientSecrets = new [] { new Secret("codemazesecret".Sha512()) },
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                AllowedScopes =  { IdentityServerConstants.StandardScopes.OpenId, "companyApi" }
            },
            new Client
            {
                ClientName = "MVC Client",
                ClientId = "mvc-client",
                AllowedGrantTypes = GrantTypes.Hybrid,
                RedirectUris = new List<string>{ "http://localhost:5003/signin-oidc" },
                RequirePkce = false,
                AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile },
                ClientSecrets = { new Secret("MVCSecret".Sha512()) },
                PostLogoutRedirectUris = new List<string> { "http://localhost:5003/signout-callback-oidc" }
        }
        };


    }
}
