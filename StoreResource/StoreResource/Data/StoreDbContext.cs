using Microsoft.EntityFrameworkCore;
using StoreResource.Models.Tables;
namespace StoreResource.Data
{
  public class StoreDbContext : DbContext
  {
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Order> Orders { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
    }
  }
}
