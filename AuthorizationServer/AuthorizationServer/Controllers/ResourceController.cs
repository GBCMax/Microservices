using AuthorizationServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuthorizationServer.Controllers
{
  [Route("api")]
  public class ResourceController : Controller
  {
    private readonly UserManager<User> _userManager;

    public ResourceController(UserManager<User> userManager)
    {
      _userManager = userManager;
    }

    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("message")]
    public async Task<IActionResult> GetMessage()
    {
      var user = await _userManager.FindByIdAsync(User.GetClaim(Claims.Subject));
      if (user is null)
      {
#pragma warning disable CS8620 // Аргумент запрещено использовать для параметра из-за различий в отношении допустимости значений NULL для ссылочных типов.
        return Challenge(
            authenticationSchemes: OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties(new Dictionary<string, string>
            {
              [OpenIddictValidationAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
              [OpenIddictValidationAspNetCoreConstants.Properties.ErrorDescription] =
                    "The specified access token is bound to an account that no longer exists."
            }));
#pragma warning restore CS8620 // Аргумент запрещено использовать для параметра из-за различий в отношении допустимости значений NULL для ссылочных типов.
      }

      return Content($"{user.UserName} has been successfully authenticated.");
    }
  }
}
