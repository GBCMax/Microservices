using AuthorizationServer.Database;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthorizationServer.Apps
{
  public class Seeder : IHostedService
  {
    private readonly IServiceProvider _serviceProvider;

    public Seeder(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      await using var scope = _serviceProvider.CreateAsyncScope();

      var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
      await context.Database.EnsureCreatedAsync(cancellationToken);

      var clientManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

      if (await clientManager.FindByClientIdAsync("store-client-id", cancellationToken) is null)
      {
        await clientManager.CreateAsync(new OpenIddictApplicationDescriptor
        {
          ApplicationType = ApplicationTypes.Native,
          ClientId = "store-client-id",
          ClientType = ClientTypes.Public,
          RedirectUris =
          {
            new Uri("https://localhost/")
          },
          Permissions =
          {
            Permissions.Endpoints.Authorization,
            Permissions.GrantTypes.Password,
            Permissions.Endpoints.Token,
            Permissions.Endpoints.Introspection,
            Permissions.GrantTypes.AuthorizationCode,
            Permissions.ResponseTypes.Code,
            Permissions.Scopes.Email,
            Permissions.Scopes.Profile,
            Permissions.Scopes.Roles,
            Permissions.Prefixes.Scope + "store-api"
          }
        }, cancellationToken);
      }

      if (await clientManager.FindByClientIdAsync("store-resource", cancellationToken) is null)
      {
        await clientManager.CreateAsync(new OpenIddictApplicationDescriptor
        {
          ClientId = "store-resource",
          ClientSecret = "846B62D0-DEF9-4215-A99D-86E6B8DAB342",
          Permissions =
          {
            Permissions.Endpoints.Introspection
          }
        }, cancellationToken);
      }

      var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

      if (await scopeManager.FindByNameAsync("store-api", cancellationToken) is null)
      {
        await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
        {
          Name = "store-api",
          Resources =
          {
            "store-resource"
          }
        }, cancellationToken);
      }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  }
}
