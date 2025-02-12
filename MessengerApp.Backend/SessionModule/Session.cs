using System.Net.WebSockets;
using MessengerApp.Backend.Models;
using MessengerApp.Backend.TCP;
using Microsoft.IdentityModel.Tokens;

namespace MessengerApp.Backend.SessionModule;
public class Session(SessionCache cache, ILogger<Session> logger)  {
    // Stores information that relates Userdata to the websocket connection
    private WebSocket _ws;
    private User user;
    public void open() {}
    public async Task StartListening() {
        var buffer = new byte[1024 * 10];
        while(_ws.State == WebSocketState.Open) {
            var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer),CancellationToken.None);
        }
    }
    public Task SocketStuff(HttpContext context) {return Task.CompletedTask;}
}