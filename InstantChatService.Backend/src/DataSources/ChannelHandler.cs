using System.Runtime.CompilerServices;
using System.Text.Json;
using Klokwork.ChatApp.DataSources.Hubs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Npgsql.Replication;

namespace Klokwork.ChatApp.DataSources.Client;
public class CreateChannelHandler(ChatHub hub) : IRequestHandler<RoutingComponent>
{
    private readonly ChatHub _hub = hub;
    public Task HandleRequestAsync(HubCollector collector, Packet packet)
    {
        var user = packet.Payload.GetProperty("authorId").GetString()!;
        string id = _hub.Create("test",user,packet.Payload.GetProperty("channelId").GetString()!);
        return Task.CompletedTask;
    }
}
public class DestroyChannelHandler : IRequestHandler<RoutingComponent>
{

    public Task HandleRequestAsync(HubCollector collector, Packet packet)
    {
        var payload = JsonSerializer.Deserialize<RoutingComponent>(packet.Payload);
        collector.GetChatHub(payload!.channelId).Destroy();
        return Task.CompletedTask;
    }
}
public class SubscribeChannelHandler : IRequestHandler<RoutingComponent>
{
    public Task HandleRequestAsync(HubCollector collector, Packet packet)
    {
        var payload = JsonSerializer.Deserialize<RoutingComponent>(packet.Payload);
        collector.GetChatHub(payload!.channelId).SubscribeUser(payload.authorId);
        return Task.CompletedTask;
    }
}
public class UnSubscribeChannelHandler : IRequestHandler<RoutingComponent>
{
    public Task HandleRequestAsync(HubCollector collector, Packet packet)
    {
        var payload = JsonSerializer.Deserialize<RoutingComponent>(packet.Payload);
        collector.GetChatHub(payload!.channelId).SubscribeUser(payload.authorId);
        return Task.CompletedTask;
    }
}