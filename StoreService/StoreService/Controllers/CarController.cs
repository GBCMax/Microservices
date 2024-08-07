using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client;
using StoreService.Models.Web.Integration.StoreResource.Requests;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;

namespace StoreService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CarController : Controller
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _httpClientFactory;
    public CarController(IHttpClientFactory httpClientFactory, IServiceProvider serviceProvider)
    {
      _httpClientFactory = httpClientFactory;
      _serviceProvider = serviceProvider;
    }

    [HttpPost("CarList")]
    public async Task<IActionResult> GetCarList([FromBody] Token _token)
    {
      using var client = _serviceProvider.GetRequiredService<HttpClient>();

      string token = "";

      switch (_token.token)
      {
        case null:
          if (HttpContext.Request.Cookies.ContainsKey("Token"))
          {
            token = HttpContext.Request.Cookies["Token"];
          }
          break;
        default:
          token = _token.token;
          break;
      }

      using var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7101/api/Car/CarList");
      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

      using var response = await client.SendAsync(request);

      var message = response.Content.ReadAsStringAsync().Result;

      return Ok(message);
    }

    [HttpPost]
    public async Task<IActionResult> AddCar(AddCarRequest request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      JsonContent jsonRequest = JsonContent.Create(request);

      using var client = _serviceProvider.GetRequiredService<HttpClient>();

      string token = "";

      if (HttpContext.Request.Cookies.ContainsKey("Token"))
      {
        token = HttpContext.Request.Cookies["Token"];
      }

      using var apiRequest = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7101/api/Car/Create");
      apiRequest.Content = jsonRequest;
      apiRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

      using var apiResponse = await client.SendAsync(apiRequest);

      var message = apiResponse.Content.ReadAsStringAsync().Result;

      return Ok(message);
    }
  }
}
