namespace StoreResource.Models.Tables
{
  public class User
  {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual List<Order> Orders { get; set; } = [];
  }
}
