using OpenIddict.Abstractions;

namespace S1.Core.Services
{
    public class DatabaseSeedWorker : IHostedService
    {
        private readonly IServiceProvider _provider;

        public DatabaseSeedWorker(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _provider.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            // Client S2
            if (await manager.FindByClientIdAsync("S2") == null)
            {
                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = "S2",
                    ClientSecret = "S2Secret",
                    DisplayName = "MES Core",
                    RedirectUris =
                    {
                        new Uri("https://localhost:5002/signin-oidc")
                    },
                    PostLogoutRedirectUris =
                    {
                        new Uri("https://localhost:5002/signout-callback-oidc")
                    },
                    Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Revocation,
                    OpenIddictConstants.Permissions.Endpoints.EndSession,

                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                    OpenIddictConstants.Permissions.ResponseTypes.Code,

                    OpenIddictConstants.Permissions.Prefixes.Scope + "openid",
                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                }
                };
                await manager.CreateAsync(descriptor);
            }

            // Client S3
            if (await manager.FindByClientIdAsync("S3") == null)
            {
                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = "S3",
                    ClientSecret = "S3Secret",
                    DisplayName = "MES Custom",
                    RedirectUris =
                    {
                        new Uri("https://localhost:5003/signin-oidc")
                    },
                    PostLogoutRedirectUris =
                    {
                        new Uri("https://localhost:5003/signout-callback-oidc")
                    },
                    Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Revocation,
                    OpenIddictConstants.Permissions.Endpoints.EndSession,

                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                    OpenIddictConstants.Permissions.ResponseTypes.Code,

                    OpenIddictConstants.Permissions.Prefixes.Scope + "openid",
                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                }
                };
                await manager.CreateAsync(descriptor);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
