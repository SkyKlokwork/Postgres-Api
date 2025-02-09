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
        var buffer = new BufferPayload(new Message("test","test","test","test")).ToBuffer();
        await test.SendAsync(buffer,WebSocketMessageType.Text,true,CancellationToken.None);
        try {
            await test.CloseAsync(WebSocketCloseStatus.NormalClosure,"Test Complete!",default);
        }
        catch (WebSocketException e) {
            Assert.Fail();
        }
    } 
}
