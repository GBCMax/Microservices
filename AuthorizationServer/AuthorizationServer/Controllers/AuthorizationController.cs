using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using AuthorizationServer.Models;
using System.Collections.Immutable;

namespace AuthorizationServer.Controllers
{
  public class AuthorizationController : Controller
  {
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IOpenIddictApplicationManager _applicationManager;

    public AuthorizationController(
        SignInManager<User> signInManager,
        UserManager<User> userManager,
        IOpenIddictApplicationManager applicationManager)
    {
      _signInManager = signInManager;
      _userManager = userManager;
      _applicationManager = applicationManager;
    }

    [HttpPost("~/connect/token"), IgnoreAntiforgeryToken, Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
      var request = HttpContext.GetOpenIddictServerRequest();

      var audience = new ImmutableArray<string>();

      switch (request!.ClientId)
      {
        case "store-client-id":
          audience = ["store-resource"];
          break;

        default:
          break;
      }

      if (request.IsPasswordGrantType())
      {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
          var properties = new AuthenticationProperties(new Dictionary<string, string>
          {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                  "The username/password couple is invalid."
          });

          return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
          var properties = new AuthenticationProperties(new Dictionary<string, string>
          {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                  "The username/password couple is invalid."
          });

          return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
                .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
                .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
                .SetClaim(Claims.PreferredUsername, await _userManager.GetUserNameAsync(user))
                .SetClaims(Claims.Audience, audience)
                .SetClaims(Claims.Role, [.. (await _userManager.GetRolesAsync(user))]);

        identity.SetScopes(new[]
        {
          Scopes.OpenId,
          Scopes.Email,
          Scopes.Profile,
          Scopes.Roles
        }.Intersect(request.GetScopes()));

        identity.SetDestinations(GetDestinations);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
      }

      throw new NotImplementedException("The specified grant type is not implemented.");
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
      switch (claim.Type)
      {
        case Claims.Name or Claims.PreferredUsername:
          yield return Destinations.AccessToken;

          if (claim.Subject.HasScope(Scopes.Profile))
            yield return Destinations.IdentityToken;

          yield break;

        case Claims.Email:
          yield return Destinations.AccessToken;

          if (claim.Subject.HasScope(Scopes.Email))
            yield return Destinations.IdentityToken;

          yield break;

        case Claims.Role:
          yield return Destinations.AccessToken;

          if (claim.Subject.HasScope(Scopes.Roles))
            yield return Destinations.IdentityToken;

          yield break;

        case "AspNet.Identity.SecurityStamp": yield break;

        default:
          yield return Destinations.AccessToken;
          yield break;
      }
    }
  }
}
