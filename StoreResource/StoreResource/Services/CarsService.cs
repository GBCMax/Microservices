using StoreResource.Interfaces.IRepo;
using StoreResource.Interfaces.IService;
using StoreResource.Models.Tables;

namespace StoreResource.Services
{
  public class CarsService : ICarsService
  {
    private readonly ICarsRepository _carsRepository;
    public CarsService(ICarsRepository carsRepository)
    {
      _carsRepository = carsRepository;
    }

    public async Task<List<Car>> GetCarList(CancellationToken ct) => await _carsRepository.GetCars(ct);
    public async Task AddCar(Car car, CancellationToken ct) => await _carsRepository.AddCar(car, ct);
  }
}
