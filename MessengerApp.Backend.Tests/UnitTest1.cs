using System.Net.WebSockets;
using System.Text.Json;
using MessengerApp.Backend.Models;
using MessengerApp.Backend.TCP;
using NetTopologySuite.Operation.Buffer;

namespace MessengerApp.Backend.Tests;

public class UnitTest1
{

    [Fact]
    public async Task SocketConnection_Test() {
        var client = new ClientWebSocket();
        await client.ConnectAsync(new Uri("ws://localhost:5130/"),CancellationToken.None);
        var buffer = new PackageData(PackageType.PACKAGE_TYPE_CHAT,PackageData.ToJsonElement(new Message("test","test","test","test"))).ToBuffer();
        for(int i = 0; i< 5; i++) {
            await client.SendAsync(buffer,WebSocketMessageType.Text,true,CancellationToken.None);
        }
        try {
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure,"Test Complete!",default);
        }
        catch (WebSocketException e) {
            Console.WriteLine(e.Message);
            Assert.Fail();
        }
    }
    [Fact]
    public async Task MultiConnection_Test() {
        var uri = new Uri("ws://localhost:5130/");
        ClientWebSocket[] simulated_clients = [new(),new(),new(),new(),new()];
        foreach (var client in simulated_clients) {
            await client.ConnectAsync(uri,CancellationToken.None);
            var buffer = new PackageData(PackageType.PACKAGE_TYPE_CHAT,JsonSerializer.SerializeToElement(new Message(Guid.NewGuid().ToString(),"test","test",client.ToString()!))).ToBuffer();
            await client.SendAsync(buffer,WebSocketMessageType.Text,true,CancellationToken.None);
        }
        foreach(var client in simulated_clients) {
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure,$"{client.ToString()} disconnected",default);
        }
    }
}
