using System.ComponentModel.DataAnnotations;

namespace StoreResource.Models.Web.Requests.Cars
{
  public class CreateCarRequest
  {
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public decimal Price { get; set; }
  }
}
