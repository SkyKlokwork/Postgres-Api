using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Klokwork.ChatApp.DataSources.RequestHandler;
using Microsoft.IdentityModel.Tokens;

namespace Klokwork.ChatApp.DataSources.Client;
public class ClientSocketMiddleware (RequestDelegate next, ClientWebSocketCache cache,RequestHandlerProvider provider) {
    private readonly RequestDelegate _next = next;
    private readonly ClientWebSocketCache _cache = cache;
    private readonly RequestHandlerProvider _provider = provider;
    private readonly byte[] _buffer = new byte[cache.options.bufferSize];
    public async Task Invoke(HttpContext context) {
        if(!context.WebSockets.IsWebSocketRequest) {
            await _next.Invoke(context);
            return;
        }
        var userId = UniqueId.CreateUniqueId();
        try {
            WebSocket client = await context.WebSockets.AcceptWebSocketAsync();
            _cache.Add(userId,client);
            var identity = new Packet(PacketType.PACKET_IDENTITY,new AuthorPayload {authorId = userId});
            await _cache.SendAsync(identity,client);
            await ReceiveFromSocket(client,new CancellationToken(false));
            await _cache.RemoveAsync(userId);
        }
        catch (Exception e) {
            Console.WriteLine($"User {userId} ran into an exception: {e.Message}");
            return;
        }
    }
    private async Task ReceiveFromSocket(WebSocket socket, CancellationToken token) {
        while(socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseSent) {
            try {
                WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(_buffer), token);
                if (result.MessageType == WebSocketMessageType.Close) {
                    break;
                }
                // temp  //
                var json = Encoding.UTF8.GetString(_buffer,0,result.Count);
                var packet = Packet.ToPacket(json);
                _provider.GetRequestHandler(packet);
                //  //
            }
            catch (Exception e) {
                continue;
            }
        }
    }
}