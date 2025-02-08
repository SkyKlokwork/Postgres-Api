using System.Diagnostics;
using MessengerApp.Backend.Models;
using MessengerApp.Backend.TCP;

namespace MessengerApp.Backend.Main;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // TODO: add a custom log formatter?
        builder.Services.AddLogging();
        builder.Logging.AddConsole();
        builder.Services.AddTransient<ServerSockets>();

    
        var app = builder.Build();

        if(Debugger.IsAttached) {
            
        }

        var socketOptions = new WebSocketOptions {
            KeepAliveInterval = TimeSpan.FromMinutes(2)
        };
        app.UseWebSockets(socketOptions);
        app.Map("/session", async (
            HttpContext context,
            ServerSockets webSocket
            ) => {
                if (context.WebSockets.IsWebSocketRequest) {
                    await webSocket.Open(context);
                } else {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            });

        app.Run();
    }
}
