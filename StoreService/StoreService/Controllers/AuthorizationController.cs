using Microsoft.AspNetCore.Mvc;
using StoreService.Models.Web;
using System.Net;

namespace StoreService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthorizationController : Controller
  {
    private readonly IServiceProvider _serviceProvider;
    public AuthorizationController(IServiceProvider provider)
    {
      _serviceProvider = provider;
    }

    [HttpPost]
    public async Task<IActionResult> Register(
      [FromBody] RegisterRequest registerRequest)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      else
      {
        using var client = _serviceProvider.GetRequiredService<HttpClient>();

        var response = await client.PostAsJsonAsync("https://localhost:10001/api/Account/Register", new { registerRequest.Email, registerRequest.Password });

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
          return Conflict("Аккаунт уже зарегистрирован");
        }

        return Ok("Регистрация прошла успешно");
      }
    }
  }
}
