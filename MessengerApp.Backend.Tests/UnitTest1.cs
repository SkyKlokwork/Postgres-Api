using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using MessengerApp.Backend.Models;
using MessengerApp.Backend.TCP;

namespace MessengerApp.Backend.Tests;

public class UnitTest1
{

    [Fact]
    public async Task SocketConnection_Test() {
        var test = new ClientWebSocket();
        await test.ConnectAsync(new Uri("ws://localhost:5130/session"),CancellationToken.None);
        var payload = new Payload(new Message("test","test","test","test")).GetPayload();
        var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(payload));
        await test.SendAsync(buffer,WebSocketMessageType.Text,true,CancellationToken.None);
        var inbuffer = new byte[1024*6];
        try {
            await test.CloseAsync(WebSocketCloseStatus.NormalClosure,"Test Complete!",default);
        }
        catch (WebSocketException e) {
            Assert.Fail();
        }
    } 
}
