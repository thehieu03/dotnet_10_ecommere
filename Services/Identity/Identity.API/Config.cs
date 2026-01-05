using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Identity.API;

public static class Config
{
    // API Scopes - Resources mà client có thể truy cập
    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("catalog", "Catalog API"),
            new ApiScope("basket", "Basket API"),
            new ApiScope("ordering", "Ordering API"),
            new ApiScope("discount", "Discount API"),
            new ApiScope("gateway", "Shopping Gateway")
        };

    // Identity Resources
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        };

    // Clients - Next.js 16 client configuration
    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            // Next.js 16 Client
            new Client
            {
                ClientId = "nextjs_client",
                ClientName = "Next.js 16 Ecommerce Client",
                
                // Allowed Grant Types - Authorization Code Flow
                AllowedGrantTypes = GrantTypes.Code,
                
                // Require PKCE for security (recommended for public clients)
                RequirePkce = true,
                
                // Client Secrets (not required for public clients like Next.js)
                RequireClientSecret = false,
                
                // Allowed Scopes
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "catalog",
                    "basket",
                    "ordering",
                    "gateway"
                },
                
                // Redirect URIs after login (Next.js default port: 3000)
                RedirectUris = new List<string>
                {
                    "http://localhost:3000/api/auth/callback",
                    "http://localhost:3000/auth/callback"
                },
                
                // Post logout redirect URIs
                PostLogoutRedirectUris = new List<string>
                {
                    "http://localhost:3000",
                    "http://localhost:3000/auth/logout"
                },
                
                // CORS allowed origins
                AllowedCorsOrigins = new List<string>
                {
                    "http://localhost:3000"
                },
                
                // Access token lifetime (default: 3600 seconds = 1 hour)
                AccessTokenLifetime = 3600,
                
                // Allow offline access (refresh tokens)
                AllowOfflineAccess = true,
                
                // Require consent (set to false for better UX)
                RequireConsent = false,
                
                // Token response type
                AlwaysIncludeUserClaimsInIdToken = true
            }
        };
}

