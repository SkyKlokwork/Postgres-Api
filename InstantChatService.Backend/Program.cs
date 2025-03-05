using System.Text.Json;
using Klokwork.ChatApp.DataSources.SignalR;
using Microsoft.AspNetCore.Http.Connections;

public class Program
{
    public static void Main(string[] args)
    {
        Environment.SetEnvironmentVariable("DOTNET_hostBuilder:reloadConfigOnChange","false");
        var builder = WebApplication.CreateBuilder(args);
        builder.Services
            .AddSignalR(huboptions => {
                huboptions.EnableDetailedErrors = true;
                huboptions.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
                huboptions.KeepAliveInterval = TimeSpan.FromMinutes(2);
                huboptions.SupportedProtocols = new List<string> { "websocket" };
            })
                .AddJsonProtocol(options => {
                    options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });
        var app = builder.Build();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.MapHub<MainHub>("/channel", options => {
            options.Transports = HttpTransportType.WebSockets | HttpTransportType.ServerSentEvents;
            options.ApplicationMaxBufferSize = 1024 * 10;
            options.TransportMaxBufferSize = 1024 * 10;
        });
        app.Run();
    }
}