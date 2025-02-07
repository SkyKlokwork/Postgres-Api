using MessengerApp.Backend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Routing.Tree;

namespace MessengerApp.Backend.ClientConnections;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // TODO: add a custom log formatter?
        builder.Services.AddLogging();
        // builder.Services.AddSingleton<>();
        builder.Services.AddScoped<Session>();

        var app = builder.Build();
        var socketOptions = new WebSocketOptions {
            KeepAliveInterval = TimeSpan.FromMinutes(2)
        };
        app.UseWebSockets(socketOptions);
        app.Map("/session", async (
            HttpContext context,
            Session session
            ) => {
                if (context.WebSockets.IsWebSocketRequest) {
                    await session.Open(context);
                } else {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            });

        app.Run();
    }
}
