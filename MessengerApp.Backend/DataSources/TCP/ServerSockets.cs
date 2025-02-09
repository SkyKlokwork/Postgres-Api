using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MessengerApp.Backend.Models;

namespace MessengerApp.Backend.TCP;
public class ServerSockets (ILogger<ServerSockets> logger){
    private WebSocket _webSocket = null!;
    public async Task Open(HttpContext context) {
        _webSocket = await context.WebSockets.AcceptWebSocketAsync();
        logger.LogDebug("Socket Handshake Complete. Opening connection");
        try{
            await Receive(_webSocket);
        }
        catch (WebSocketException e) {
            logger.LogError($"Something went wrong with the connection, {e}");
        }
    }
    public async Task Receive(WebSocket web) {
        var buffer = new byte[1024*10];
        while (web.State == WebSocketState.Open) {
            var result = await web.ReceiveAsync(new ArraySegment<byte>(buffer),default);
            if (result.MessageType == WebSocketMessageType.Close) {
                await web.CloseAsync(result.CloseStatus!.Value,result.CloseStatusDescription,default);
                logger.LogDebug("Closing connection! {s}",result.CloseStatusDescription);
                return;
            }
            var output = BufferPayload.FromBuffer(buffer[..result.Count]);
            var message = JsonSerializer.Deserialize<Message>(output.GetBufferContent());
            logger.LogDebug("New Message: {s}",message.Content);
        }
    }
}