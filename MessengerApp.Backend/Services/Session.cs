using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using MessengerApp.Backend.Models;

namespace MessengerApp.Backend.Services;
public class Session(ILogger<Session> logger) : ISession {
    // m_ is a member variable
    private WebSocket m_session = null!;
    public async Task Open(HttpContext context) {
        m_session = await context.WebSockets.AcceptWebSocketAsync();
        try {
            await Receive(m_session);
        } 
        catch (WebSocketException e) {
            logger.LogError(e,"error");
        }
    }
    public async Task Close(string reason) {
        await m_session.CloseAsync(WebSocketCloseStatus.PolicyViolation,reason,default);
    }
    public async Task Receive(WebSocket session){
        var buffer = new byte[1024 * 4];
        while(session.State == WebSocketState.Open) {
            var packet = await session.ReceiveAsync(new ArraySegment<byte>(buffer),CancellationToken.None);
            if(packet.MessageType == WebSocketMessageType.Close) {
                await m_session.CloseAsync(packet.CloseStatus!.Value,packet.CloseStatusDescription,CancellationToken.None);
                return;
            }
            
        }
    }
    public async Task Send(Message message) {
        var convertedMessage = JsonSerializer.Serialize<Message>(message);
        var payload = new ArraySegment<byte>(Encoding.UTF8.GetBytes(convertedMessage));
        await m_session.SendAsync(payload,WebSocketMessageType.Text,true,CancellationToken.None);
    }
    // replace for Images and Files?
    [Experimental("Unimplemented")]
    private async Task Send(Object obj) {
        await m_session.SendAsync(new ArraySegment<byte>(new byte[1024 * 4]),WebSocketMessageType.Binary,true,CancellationToken.None);
    }   
}