using System.Net.WebSockets;
using System.Text.Json.Nodes;

namespace MessengerApp.Backend.TCP;
public interface IWSHandler  {
    public Task SendPackageAsync(WebSocket socket, JsonObject content);
}