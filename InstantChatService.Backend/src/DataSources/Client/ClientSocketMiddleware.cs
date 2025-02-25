
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
namespace Klokwork.ChatApp.DataSources.Client;
public class ClientSocketMiddleware (
    RequestDelegate next,
    ClientCache cache
) {
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
        catch (Exception) {
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
                var output = new MessageRequest();
                var packet = Packet.ToPacket(Encoding.UTF8.GetString(buffer[..result.Count]));
                var data = packet.Payload.Deserialize<TextChat>();
                await output.HandleRequestAsync(data!);
            }
            catch (Exception e) {
                Console.WriteLine($"{e}");
                continue;
            }
        }
    }
}