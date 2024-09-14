namespace StoreService.Models.WebSocketModels
{
  public record WsMessage(
  Guid Sender,
  object Message);
}
