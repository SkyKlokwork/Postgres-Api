using System.Text.Json.Nodes;

namespace MessengerApp.Backend.TCP;
public interface IWSocketHandler {
    public Task Open(HttpContext context, Func<string,Task> Callback);
    public Task SendData(JsonObject obj);
}