using System.Net.WebSockets;
using MessengerApp.Backend.Models;

namespace MessengerApp.Backend.TCP;
public class WSocketHandler(ILogger<WSocketHandler> logger) {
    private WebSocket _ws = null!;
    public async Task SendAsync(Message message) {
        await _ws.SendAsync(
            new BufferPayload(message).ToBuffer(),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }
    public async Task<BufferPayload?> ReceiveAsync() {
        return null;
    }
    public async Task Listner() {

    }
}