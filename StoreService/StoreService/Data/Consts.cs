using StoreService.Models.WebSocketModels;
using System.Collections.Frozen;

namespace StoreService.Data
{
    public static class Consts
  {
    public static FrozenDictionary<string, Type> WS_MSG_TYPES = new Dictionary<string, Type>
    {
      { "ws-msg-hello", typeof (WsMessageHello) },
    }.ToFrozenDictionary();
  }
}
