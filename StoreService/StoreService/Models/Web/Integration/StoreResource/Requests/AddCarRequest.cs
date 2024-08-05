using System.ComponentModel.DataAnnotations;

namespace StoreService.Models.Web.Integration.StoreResource.Requests
{
  public class AddCarRequest
  {
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public decimal Price { get; set; }
  }
}
