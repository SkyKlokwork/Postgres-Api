using System.Net.WebSockets;
using System.Text.Json;
using MessengerApp.Backend.Models;
using MessengerApp.Backend.TCP;

namespace MessengerApp.Backend.Tests;

public class UnitTest1
{

    [Fact]
    public async Task SocketConnection_Test() {
        var client = new ClientWebSocket();
        await client.ConnectAsync(new Uri("ws://localhost:5130/"),CancellationToken.None);
        var buffer = new BufferContent(BufferDataEnum.BUFFER_TYPE_CHAT,JsonSerializer.SerializeToElement(new Message("test","test","test","test"))).ToBuffer();
        for(int i = 0; i< 5; i++) {
            await client.SendAsync(buffer,WebSocketMessageType.Text,true,CancellationToken.None);
        }
        try {
            await Task.Delay(100);
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure,"Test Complete!",default);
        }
        catch (WebSocketException e) {
            Console.WriteLine(e.Message);
            Assert.Fail();
        }
    } 
}
