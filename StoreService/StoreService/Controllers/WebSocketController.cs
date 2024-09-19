using Microsoft.AspNetCore.Mvc;
using StoreService.ImitationCarTravel;
using StoreService.Models.WebSocketModels;
using StoreService.Servers;

namespace StoreService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class WebSocketController : Controller
  {
    public WebSocketImpl p_ws;
    public WebSocketController()
    {
      p_ws = new WebSocketImpl();
    }

    [HttpGet("connect")]
    public virtual async Task<IResult> StartWebSocketAsync(
    CancellationToken _ct)
    {
      if (!Request.HttpContext.WebSockets.IsWebSocketRequest)
      {
        Console.WriteLine("Not a web socket request");
        return Results.Empty;
      }

      Console.WriteLine($"Establishing WS connection for user '{0}'...");

      using var websocket = await Request.HttpContext.WebSockets.AcceptWebSocketAsync();
      _ = await p_ws.AcceptSocketAsync(0, websocket);
      Console.WriteLine($"WS connection for user '{0}' is closed");

      return Results.Empty;
    }

    [HttpGet("connectToCar")]
    public async Task<IResult> StartTravel(
    CancellationToken _ct)
    {
      Console.WriteLine($"Starting engine...");

      return Results.Ok(Travel.StartTravel().Result);
    }
  }
}
