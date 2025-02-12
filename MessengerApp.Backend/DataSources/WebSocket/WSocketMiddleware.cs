using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MessengerApp.Backend.TCP;
public class WSocketMiddleware {
    private readonly RequestDelegate _next;
    private readonly WSocketHandler _sh;

    public WSocketMiddleware(RequestDelegate next, WSocketHandler socketHandler) {
        _next = next;
        _sh = socketHandler;
    }

    public async Task Invoke(HttpContext context) {
        if(!context.WebSockets.IsWebSocketRequest) {
            await _next.Invoke(context);
            return;
        }
        await _sh.Open(context, async (string jsonString) 
            => {
                var json = string.IsNullOrEmpty(jsonString) 
                    ? throw new Exception() : JsonNode.Parse(jsonString);
                await thing(json!);
            });
    }
    public Task thing(JsonNode test) {
        Console.WriteLine($"Message: {test["DATA"]!.GetValue<string>()}");
        return Task.CompletedTask;
    }
}