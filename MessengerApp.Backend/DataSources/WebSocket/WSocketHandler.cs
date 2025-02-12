using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MessengerApp.Backend.Models;
using Microsoft.AspNetCore.Components;

namespace MessengerApp.Backend.TCP;
public class WSocketHandler(ILogger<WSocketHandler> logger) {
    private WebSocket _ws = null!;
    // Callback might not work here
    // might lead to undefined behaviour?
    public async Task Open(HttpContext context, Func<string,Task> Callback) {
        _ws = await context.WebSockets.AcceptWebSocketAsync();
        try {
            await Receiver(Callback);
        }
        catch (WebSocketException e) {
            logger.LogError("Websocket connection error: {e}",e);
            return;
        }
    }
    public async Task Receiver(Func<string,Task> CallBack) {
        var buffer = new byte[1024 * 10];
        while(_ws.State == WebSocketState.Open) {
            var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer),CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close) {
                await _ws.CloseOutputAsync(result.CloseStatus!.Value,result.CloseStatusDescription,default);
                return;
            }
            // possibly code to run decoding
            // might instead move directly to bytes
            var data = Encoding.UTF8.GetString(buffer[..result.Count]);
            // this callback is what moves the data out of the while loop
            // it also runs everytime a new result is received making it exactly what is needed
            // might be a more eloquent way of doing this however?
            await CallBack(data);
        }
    }

    public Task SendData(JsonObject obj)
    {
        throw new NotImplementedException();
    }
}