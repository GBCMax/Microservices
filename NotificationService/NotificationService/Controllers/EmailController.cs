using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class EmailController : Controller
  {
    public EmailController()
    {

    }

    [HttpGet("GetMessage")]
    public async Task<IActionResult> GetMessage()
    {
      return Ok($"Callback from notify service");
    }

    [HttpPost("Notify")]
    public async Task<ActionResult> Notify()
    {
      return Ok();
    }
  }
}
