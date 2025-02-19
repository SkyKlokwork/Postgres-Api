using System.Net.WebSockets;
using MessengerApp.Backend.SessionModule;
using MessengerApp.Backend.TCP;
using Microsoft.IdentityModel.Tokens;

namespace MessengerApp.Backend;

public class OnConnection {
    // this class is starting to take too many dependencies
    // may be good to re-evaluate the purposes of each dependency and where it needs to be
    private readonly RequestDelegate _next;
    private readonly WSHandler _handler;
    private readonly PackageEvents _package;
    // SessionCache currently is tracking a singular Websocket connection and a string user name associated with that connection.
    // in the future this will change to tracking multiple connections which are associated with one user
    // might need the app to receive push notifications somehow
    private readonly SessionCache _cache;
    private WebSocket ws = null!;
    public OnConnection(RequestDelegate next, WSHandler handler,PackageEvents package,SessionCache cache) {
        _next = next;
        _handler = handler;
        _cache = cache;
        _package = package;
        _package.PackageReceivedEvent += ReceivedPackageHandler;
    }

    public async Task Invoke(HttpContext context) {
        if(!context.WebSockets.IsWebSocketRequest) {
            // await _next.Invoke(context);
            return;
        }
        ws = await WSHandler.OpenSocket(context);
        await _handler.SocketListener(ws);
        await _cache.AddSocket(UniqueId.CreateUniqueId(),ws);
    }

    private void ReceivedPackageHandler(object? sender, PackageReceivedEventArgs e)
    {
        Console.WriteLine($"Package Content: {e.package.GetJson().ToString()}");
    }
}