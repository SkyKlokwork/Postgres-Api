using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MessengerApp.Backend.ClientConnections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit.Sdk;

namespace MessengerApp.Backend.Tests;

public class UnitTest1
{

    [Fact]
    public async Task SocketConnection_Test() {
        var ws = new ClientWebSocket();
        await ws.ConnectAsync(new Uri("ws://localhost:5130/chat"),CancellationToken.None);
        try {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure,"Test Complete",default);
            Assert.True(true, "Connection Completed");
            return;
        } catch (WebSocketException e) {
            Assert.Fail($"Connection Test Failed! {e}");
        }
    } 
}
