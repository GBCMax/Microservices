namespace StoreResource.Models.Tables
{
  public class Order
  {
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int CarId { get; set; }
    public Car? Car { get; set; }
  }
}
