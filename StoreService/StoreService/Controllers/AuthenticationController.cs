using Microsoft.AspNetCore.Mvc;
using System.Net;
using OpenIddict.Client;
using StoreService.Models.Web;

namespace StoreService.Controllers
{
    [ApiController]
  [Route("api/[controller]")]
  public class AuthenticationController : Controller
  {
    private readonly IServiceProvider _serviceProvider;
    public AuthenticationController(IServiceProvider provider)
    {
      _serviceProvider = provider;
    }

    [HttpPost]
    public async Task<IActionResult> Login(
      [FromBody] LoginRequest loginRequest)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      else
      {
        using var client = _serviceProvider.GetRequiredService<HttpClient>();

        var response = await client.PostAsJsonAsync("https://localhost:10001/api/Account/Login", new { loginRequest.Email, loginRequest.Password });

        var service = _serviceProvider.GetRequiredService<OpenIddictClientService>();

        var result = await service.AuthenticateWithPasswordAsync(new()
        {
          Username = loginRequest.Email,
          Password = loginRequest.Password
        });

        var token = result.AccessToken;

        HttpContext.Response.Cookies.Append("Token", token);

        return response.StatusCode != HttpStatusCode.NotFound
          ? response.StatusCode == HttpStatusCode.Conflict
            ? BadRequest("Неверный логин или пароль")
            : Ok("Аутентификация пройдена")
          : NotFound("Аккаунт не найден");
      }
    }
  }
}
