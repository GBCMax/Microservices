using StoreResource.Models.Tables;

namespace StoreResource.Interfaces.IRepo
{
  public interface ICarsRepository
  {
    Task<List<Car>> GetCars(CancellationToken ct);
    Task AddCar(Car car, CancellationToken ct);
  }
}
