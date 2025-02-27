
using Klokwork.ChatApp.DataSources.Hubs;

namespace Klokwork.ChatApp.DataSources.Client;
public class TextHandler : IRequestHandler<MessagePayload>
{
    public async Task HandleRequestAsync(HubCollector collector, Packet packet)
    {
        try {
        string key = packet.Payload.GetProperty("channelId").GetString()!;
        ChatHub selectedhub = collector.GetChatHub(key);
        await selectedhub.broadcast(packet: packet);
        }
        catch(Exception e) {
            Console.WriteLine($"Exception Caught:{e.Message}. Message: {packet.Payload.GetProperty("message").GetString()}");
            return;
        }
    }
}