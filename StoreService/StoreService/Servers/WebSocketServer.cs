using StoreService.Models.WebSocketModels;
using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Text.Json;
using System.Text;
using StoreService.Data;
using System.Text.Json.Serialization;
using StoreService.JsonCtx;

namespace StoreService.Servers
{
  public class WebSocketServer
  {
    private readonly Subject<WebSocketSession> p_clientConnectedFlow = new();
    private readonly ConcurrentDictionary<int, WebSocketSession> p_sessions = new();
    private readonly Subject<WsMessage> p_incomingMsgs = new();
    private int p_sessionsCount = 0;
    private readonly int p_maxConnectionTime = 60 * 60 * 1000;
    private readonly JsonSerializerContext p_jsonCtx = WebSocketJsonCtx.Default;
    private readonly IReadOnlyDictionary<string, Type> p_messageTypes = Consts.WS_MSG_TYPES;
    private readonly Dictionary<Type, string> p_messageTypesReverse = new Dictionary<Type, string>();
    private readonly Subject<byte[]> p_broadcastQueueSubj = new();

    public WebSocketServer()
    {
      foreach (var msgType in p_messageTypes)
      {
        p_messageTypesReverse[msgType.Value] = msgType.Key;
      }
    }

    public IObservable<WebSocketSession> ClientConnected => p_clientConnectedFlow;
    public IObservable<WsMessage> IncomingMessages => p_incomingMsgs;
    public IReadOnlyList<WebSocketSession> Sessions => new List<WebSocketSession>(p_sessions.Values);
    public async Task<bool> AcceptSocketAsync(Guid id, WebSocket webSocket)
    {
      if (webSocket.State is not WebSocketState.Open)
      {
        return false;
      }

      var session = new WebSocketSession(id, webSocket);
      using var semaphore = new SemaphoreSlim(0, 1);
      using var scheduler = new EventLoopScheduler();

      scheduler.ScheduleAsync(async (_s, _ct) => await CreateNewLoopAsync(session, semaphore));

      try
      {
        await semaphore.WaitAsync(new CancellationToken());
      }
      catch (OperationCanceledException) { }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }

      return true;
    }

    public async Task<int> BroadcastMsgAsync<T>(T _msg, CancellationToken _ct) where T : notnull
    {
      var buffer = CreateWsMessage(_msg);

      return await BroadcastMsgAsync(buffer, _ct);
    }

    public async Task<int> BroadcastMsgAsync(byte[] _msg, CancellationToken _ct)
    {
      var totalSent = 0;
      foreach (var (index, session) in p_sessions)
      {
        try
        {
          if (session.WebSocket.State == WebSocketState.Open)
          {
            await session.WebSocket.SendAsync(_msg, WebSocketMessageType.Text, true, _ct);
            totalSent++;
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Can't send msg to socket '{index}': {ex}");
        }
      }
      return totalSent;
    }

    public void PostBroadcastMsg<T>(T _msg) where T : notnull
    {
      var buffer = CreateWsMessage(_msg);
      p_broadcastQueueSubj.OnNext(buffer);
    }

    private async Task CreateNewLoopAsync(
      WebSocketSession _session,
      SemaphoreSlim _completeSignal)
    {
      var session = _session;
      var sessionIndex = Interlocked.Increment(ref p_sessionsCount);
      p_sessions.TryAdd(sessionIndex, session);

      using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(p_maxConnectionTime));

      WebSocketReceiveResult? receiveResult = null;

      var buffer = ArrayPool<byte>.Shared.Rent(100 * 1024);

      try
      {
        p_clientConnectedFlow.OnNext(session);

        receiveResult = await session.WebSocket.ReceiveAsync(buffer, cts.Token);

        while (!receiveResult.CloseStatus.HasValue && !cts.IsCancellationRequested)
        {
          cts.CancelAfter(TimeSpan.FromMilliseconds(p_maxConnectionTime));

          try
          {
            if (TryParseWsMsg(buffer[..receiveResult.Count], out var msg, out var msgType))
              p_incomingMsgs.OnNext(new WsMessage(_session.Id, msg));
          }
          finally
          {
            receiveResult = await session.WebSocket.ReceiveAsync(buffer, cts.Token);
          }
        }
      }
      catch (OperationCanceledException)
      {
        // don't care
      }
      catch (WebSocketException wsEx) when (wsEx.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
      {
        // don't care
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error occured in loop: {ex}");
      }
      finally
      {
        ArrayPool<byte>.Shared.Return(buffer, false);
        p_sessions.TryRemove(sessionIndex, out _);
      }

      try
      {
        if (session.WebSocket.State == WebSocketState.Open)
        {
          if (receiveResult is not null)
            await session.WebSocket.CloseAsync(receiveResult.CloseStatus ?? WebSocketCloseStatus.NormalClosure, receiveResult.CloseStatusDescription, CancellationToken.None);
          else
            await session.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, $"Closed normally (session: '{sessionIndex}')", CancellationToken.None);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error occured while closing websocket: {ex}");
      }

      _completeSignal.Release();
    }

    public async Task SendMsgAsync<T>(WebSocketSession _session, T _msg, CancellationToken _ct) where T : notnull
    {
      var buffer = CreateWsMessage(_msg);

      try
      {
        if (_session.WebSocket.State == WebSocketState.Open)
          await _session.WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, _ct);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Can't send msg to socket: {ex}");
      }
    }

    public async Task SendMsgAsync<T>(IEnumerable<WebSocketSession> _sessions, T _msg, CancellationToken _ct) where T : notnull
    {
      var buffer = CreateWsMessage(_msg);

      foreach (var session in _sessions)
      {
        try
        {
          if (session.WebSocket.State == WebSocketState.Open)
            await session.WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, _ct);
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Can't send msg to socket: {ex}");
        }
      }
    }

    private byte[] CreateWsMessage<T>(T _msg) where T : notnull
    {
      var type = typeof(T);
      if (!p_messageTypesReverse.TryGetValue(type, out var typeSlug))
        throw new InvalidOperationException($"Unknown type '{type}'");

      var baseMsg = new WsBaseMessage(typeSlug, _msg);
      var json = JsonSerializer.Serialize(baseMsg, typeof(WsBaseMessage), p_jsonCtx);
      return Encoding.UTF8.GetBytes(json);
    }


    private bool TryParseWsMsg(
    byte[] _msg,
    [NotNullWhen(true)] out object? _payload,
    [NotNullWhen(true)] out Type? _payloadType)
    {
      _payload = null;
      _payloadType = null;
      try
      {
        var json = Encoding.UTF8.GetString(_msg);
        if (JsonSerializer.Deserialize(json, typeof(WsBaseMessage), p_jsonCtx) is not WsBaseMessage incomingBaseMsg)
          return false;

        if (!p_messageTypes.TryGetValue(incomingBaseMsg.Type, out var type))
          return false;

        _payload = ((JsonElement)incomingBaseMsg.Payload).Deserialize(type, p_jsonCtx);
        if (_payload == null)
          return false;

        _payloadType = type;
        return true;
      }
      catch
      {
        return false;
      }
    }

  }
}
