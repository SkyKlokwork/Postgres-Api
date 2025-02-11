using System.Net.WebSockets;
using System.Text.Json;
using MessengerApp.Backend.Models;
using MessengerApp.Backend.SessionModule;

namespace MessengerApp.Backend.TCP;
public class WSocketConnection (ILogger<WSocketConnection> logger) : IWSocketConnecton{
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
    public async Task Close(string reason) {
        await _webSocket.CloseAsync(WebSocketCloseStatus.PolicyViolation, reason, default);
    }
    public async Task Send(Message message) {
        var buffer = new BufferPayload(message).ToBuffer();
        await _webSocket.SendAsync(buffer,WebSocketMessageType.Text,true,CancellationToken.None);
    }
    public async Task Receive(WebSocket web) {
        var buffer = new byte[1024*10];
        while (web.State == WebSocketState.Open) {
            var result = await web.ReceiveAsync(new ArraySegment<byte>(buffer),default);
            if (result.MessageType == WebSocketMessageType.Close) {
                await web.CloseOutputAsync(result.CloseStatus!.Value,result.CloseStatusDescription,default);
                logger.LogDebug("Closing connection! {s}",result.CloseStatusDescription);
                return;
            }
            var output = BufferPayload.FromBuffer(buffer[..result.Count]);
            var message = JsonSerializer.Deserialize<Message>(output.GetBufferContent());
            logger.LogDebug("New Message: {s}",message.Content);
        }
    }
    // need someway to return received info out of the socket.
    // current implementation makes it impossible to get it from socket to say the session 
    public Task Listner()
    {
        throw new NotImplementedException();
    }
}