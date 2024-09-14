using Microsoft.AspNetCore.Mvc;
using StoreService.Extensions;
using StoreService.Models.WebSocketModels;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Reactive;
using System.Reactive.Linq;

namespace StoreService.Servers
{
    public class WebSocketImpl
    {
        private readonly WebSocketServer p_wsServer;
        private readonly ConcurrentDictionary<Guid, int> p_users = new();
        public WebSocketImpl()
        {
            p_wsServer = new WebSocketServer();
            p_wsServer.ClientConnected
              .SelectAsync(async (_session, _ct) =>
              {
                  try
                  {
                      await p_wsServer.SendMsgAsync(_session, new WsMessageHello("Превет, медвед"), _ct);
                  }
                  catch (Exception ex)
                  {
                      Console.WriteLine($"Can't send hello msg: {ex}");
                  }
              })
              .Subscribe(new CancellationToken());

            p_wsServer.IncomingMessages
            .Subscribe(_msg =>
            {
                var user = p_users.GetValueOrDefault(_msg.Sender);
                Console.WriteLine($"**Got msg** of type __{_msg.Message.GetType().Name}__ from user __{user}__");
            }, new CancellationToken());

        }

        public async Task<bool> AcceptSocketAsync(int _userId, WebSocket _webSocket)
        {
            var id = Guid.NewGuid();
            p_users[id] = _userId;

            return await p_wsServer.AcceptSocketAsync(id, _webSocket);
        }

        public int? GetUserIdByWebSocketSession(WebSocketSession _session)
        {
            if (p_users.TryGetValue(_session.Id, out var userId))
                return userId;

            return null;
        }

        public void PostBroadcastMessage<T>(T _msg) where T : notnull => p_wsServer.PostBroadcastMsg(_msg);

        public async Task SendMessageAsync<T>(WebSocketSession _session, T _msg, CancellationToken _ct) where T : notnull => await p_wsServer.SendMsgAsync(_session, _msg, _ct);

        public async Task SendMessageAsync<T>(int _userId, T _msg, CancellationToken _ct) where T : notnull
        {
            var guids = p_users
              .Where(_ => _.Value == _userId)
              .Select(_ => _.Key)
              .ToHashSet();

            var sessionsEE = p_wsServer.Sessions
              .Where(_ => guids.Contains(_.Id));

            await p_wsServer.SendMsgAsync(sessionsEE, _msg, _ct);
        }

    }
}
