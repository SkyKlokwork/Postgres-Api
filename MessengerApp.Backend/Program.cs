namespace MessengerApp.Backend.Main;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // TODO: add a custom log formatter?
        builder.Services.AddLogging();


        var app = builder.Build();
        var socketOptions = new WebSocketOptions {
            KeepAliveInterval = TimeSpan.FromMinutes(2)
        };
        app.UseWebSockets(socketOptions);
        app.Map("/session", async (
            HttpContext context
            ) => {
                if (context.WebSockets.IsWebSocketRequest) {
                    await Task.Delay(100);
                } else {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            });

        app.Run();
    }
}
