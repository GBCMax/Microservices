using System.Text.Json.Serialization;

namespace StoreService.Models.Web.Integration.StoreResource.Requests
{
  [JsonSerializable(typeof(Token))]
  public class Token
  {
    public string? token { get; set; } = string.Empty;
  }
}
