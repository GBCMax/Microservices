using StoreResource.Models.Tables;

namespace StoreResource.Interfaces.IService
{
  public interface ICarsService
  {
    Task<List<Car>> GetCarList(CancellationToken ct);
    Task AddCar(Car car, CancellationToken ct);
  }
}
