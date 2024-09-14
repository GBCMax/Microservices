using System.Net.WebSockets;

namespace StoreService.Models.WebSocketModels
{
  public record WebSocketSession(
    Guid Id,
    WebSocket WebSocket);
}
