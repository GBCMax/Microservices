using StoreService.Models.WebSocketModels;
using System.Text.Json.Serialization;

namespace StoreService.JsonCtx
{
  [JsonSourceGenerationOptions(
  PropertyNameCaseInsensitive = true,
  PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
  [JsonSerializable(typeof(WsBaseMessage))]
  [JsonSerializable(typeof(WsMessageHello))]
  [JsonSerializable(typeof(WsMessageSpeedUpdate))]
  public partial class WebSocketJsonCtx : JsonSerializerContext { }
}
