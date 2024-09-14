namespace StoreService.Models.WebSocketModels
{
  public record WsBaseMessage(
  string Type,
  object Payload);
}
