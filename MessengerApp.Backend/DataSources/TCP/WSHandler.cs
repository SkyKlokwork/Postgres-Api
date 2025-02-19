using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MessengerApp.Backend.Models;

namespace MessengerApp.Backend.TCP;
public class WSHandler(PackageEvents package) : IWSHandler {
    // Should this class handle everything to do with a particular socket?
    private WebSocket ws;
    public static async Task<WebSocket> OpenSocket(HttpContext context) {
        return await context.WebSockets.AcceptWebSocketAsync();
    }
    // temp
    // TODO: make sure this doesn't become perm
    public async Task SendPackageAsync(WebSocket socket, JsonObject content) {
        var jsonelement = content.GetValue<JsonElement>();
        await socket.SendAsync(
            new PackageData(PackageType.PACKAGE_TYPE_CHAT,jsonelement).ToBuffer(), 
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }

    public async Task SocketListener(WebSocket socket) {
        var buffer = new byte[1024*2];
        while(socket.State == WebSocketState.Open) {
            var result = await socket.ReceiveAsync(buffer,CancellationToken.None);
            if(result.MessageType == WebSocketMessageType.Close) {
                await socket.CloseOutputAsync(result.CloseStatus!.Value,result.CloseStatusDescription,default);
                return;
            }
            var rawjson = Encoding.UTF8.GetString(buffer,0,result.Count);
            var repackagedData = PackageData.FromJson(rawjson);
            package.ReceivedPackage(repackagedData);
        }
    }
}