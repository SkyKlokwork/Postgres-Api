using System.Diagnostics;
using System.Threading.Channels;
using MessengerApp.Backend.Models;
using MessengerApp.Backend.SessionModule;
using MessengerApp.Backend.TCP;

namespace MessengerApp.Backend.Main;

public class Program
{
    public static void Main(string[] args)
    {
        Environment.SetEnvironmentVariable("DOTNET_hostBuilder:reloadConfigOnChange","false");
        var builder = WebApplication.CreateBuilder(args);
        // TODO: add a custom log formatter?
        builder.Services.AddLogging();
        builder.Logging.AddConsole();
        builder.Services
            .AddTransient<WSocketHandler>();
        var app = builder.Build();
        if(Debugger.IsAttached) {
            
        }
        var socketOptions = new WebSocketOptions {
            KeepAliveInterval = TimeSpan.FromMinutes(2)
        };
        app.UseWebSockets(socketOptions);
        app.UseMiddleware<WSocketMiddleware>();
        app.Run();
    }
}
