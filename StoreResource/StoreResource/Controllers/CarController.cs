using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using StoreResource.Interfaces.IService;
using StoreResource.Models.Tables;
using StoreResource.Models.Web.Requests.Cars;

namespace StoreResource.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CarController : Controller
  {
    private readonly ICarsService _carsService;
    public CarController(ICarsService carsService)
      => _carsService = carsService;

    [HttpGet("CarList")]
    [Authorize]
    public async Task<IActionResult> GetAllCars(CancellationToken ct)
      => Ok(await _carsService.GetCarList(ct));

    [HttpPost("Create")]
    [Authorize]
    public async Task<IActionResult> AddCar(
      [FromBody] CreateCarRequest request, 
      CancellationToken ct)
    {
      Console.WriteLine($"Try to add new car: {request.Name}");

      if (!ModelState.IsValid)
      {
        Console.WriteLine($"Bad request to add car: {request.Name}, {request.Description}, {request.Price}");

        return BadRequest(ModelState);
      }

      var car = new Car()
      {
        Name = request.Name,
        Description = request.Description,
        Price = request.Price
      };

      await _carsService.AddCar(car, ct);

      return Ok($"Car: {request.Name} added");
    }
  }
}
