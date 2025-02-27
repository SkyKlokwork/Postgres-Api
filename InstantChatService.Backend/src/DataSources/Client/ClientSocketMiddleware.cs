
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Klokwork.ChatApp.DataSources.Hubs;
using Microsoft.IdentityModel.Tokens;
namespace Klokwork.ChatApp.DataSources.Client;
public class ClientSocketMiddleware (
    RequestDelegate next,
    ClientCache cache,
    HubCollector collector,
    CreateChannelHandler createChannel
) {
    // TODO: remove this later
    // its only for testing and can probably have its logic taken on by another class
    private readonly HubCollector _collector = collector;
    private readonly CreateChannelHandler _createChannel = createChannel;
    private readonly DestroyChannelHandler _destroyChannel = new ();
    private readonly SubscribeChannelHandler _subscribeChannel = new ();
    private readonly UnSubscribeChannelHandler _unsubscribeChannel = new ();
    private readonly TextHandler _textHandler = new ();
    // ----------------------------------------------------------------------- 

    private readonly RequestDelegate _next = next;
    private readonly ClientCache _cache = cache;
    private readonly byte[] buffer = new byte[cache.Options.bufferSize];
    public async Task Invoke(HttpContext context) {
        if (!context.WebSockets.IsWebSocketRequest) {
            await _next.Invoke(context);
            return;
        }
        try {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var key = UniqueId.CreateUniqueId();
            _cache.Add(key,webSocket);
            await Listener(key,webSocket);
        }
        catch (Exception e) {
            Console.WriteLine($"{e}");
            return;
        }
    }
    public async Task Listener(string key, WebSocket websocket) {
        while (websocket.State == WebSocketState.Open || websocket.State == WebSocketState.CloseSent) {
            try {
                WebSocketReceiveResult result = await websocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close) {
                    await _cache.RemoveAsync(key,websocket);
                    return;
                }
                var packet = Packet.ToPacket(Encoding.UTF8.GetString(buffer[..result.Count]))!;
                await RequestHandlerProviderAsync(packet);
            }
            catch (Exception e) {
                Console.WriteLine($"{e}");
                continue;
            }
        }
    }
    // purely for testing
    // probably don't need them all to await anyways
    private async Task RequestHandlerProviderAsync(Packet packet) {
        switch(packet.Type) {
            case PacketType.PACKET_CREATE_SERVER:
                await _createChannel.HandleRequestAsync(_collector, packet);
            break;
            case PacketType.PACKET_DESTROY_SERVER:
                await _destroyChannel.HandleRequestAsync(_collector,packet);
            break;
            case PacketType.PACKET_SUBSCRIBE:
                await _subscribeChannel.HandleRequestAsync(_collector,packet);
            break;
            case PacketType.PACKET_UNSUBSCRIBE:
                await _unsubscribeChannel.HandleRequestAsync(_collector,packet);
            break;
            case PacketType.PACKET_MESSAGE:
                await _textHandler.HandleRequestAsync(_collector,packet);
            break;
            default:
            break;
        }
    }
}