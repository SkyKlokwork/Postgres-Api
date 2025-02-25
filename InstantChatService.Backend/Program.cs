using System.Net.Sockets;
using Klokwork.ChatApp.DataSources.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;

public class Program
{
    public static void Main(string[] args)
    {
        Environment.SetEnvironmentVariable("DOTNET_hostBuilder:reloadConfigOnChange","false");
        var builder = WebApplication.CreateBuilder(args);
        builder.Services
            .AddOptions()
            .Configure<ClientSocketOptions>(options => {})
            .AddSingleton<ClientCache>();
        var app = builder.Build();
        app.UseHttpsRedirection();
        var wsoptions = new WebSocketOptions {
            KeepAliveInterval = TimeSpan.FromMinutes(1),
            
        };
        app.UseWebSockets(wsoptions);
        app.UseClientManager();
        app.UseRouting();
        app.Run();
    }
}