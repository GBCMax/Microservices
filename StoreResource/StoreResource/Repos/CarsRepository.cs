using Microsoft.EntityFrameworkCore;
using StoreResource.Data;
using StoreResource.Interfaces.IRepo;
using StoreResource.Models.Tables;
using StoreResource.Models.Web.Requests.Cars;

namespace StoreResource.Repos
{
  public class CarsRepository : ICarsRepository
  {
    private StoreDbContext _context;
    public CarsRepository(StoreDbContext context)
    {
      _context = context;
    }

    public async Task<List<Car>> GetCars(CancellationToken ct)
      => await _context.Cars
                       .AsNoTracking()
                       .ToListAsync(ct);

    public async Task AddCar(
      Car car,
      CancellationToken ct)
    {
      await _context.Cars
                    .AddAsync(car, ct);

      await _context.SaveChangesAsync();
    }
  }
}
