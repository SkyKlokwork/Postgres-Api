using System.Collections.Concurrent;
using System.Security.Cryptography;
using Klokwork.ChatApp.DataSources.Client;
using Klokwork.ChatApp.SignalR;
using Microsoft.IdentityModel.Tokens;

namespace Klokwork.ChatApp.DataSources.Hubs;
/// <summary>
/// A class created specifically for testing out how a channel system may work.
/// This should not be the final iteration and I will hurt myself if I keep it this way
/// </summary>
/// <param name="cache"></param>
/// <param name="collector"></param>
public class ChatHub (ClientCache cache,HubCollector collector) {
    private string name = null!;
    private string channelId {get; set;} = null!;
    private readonly HubCollector _collector = collector;
    private readonly ClientCache _cache = cache;
    private List<string> _subed_users = new ();
    // probably better way of doing this buf don't care right now
    public string Create(string name,string creator,string channelid) {
        channelId = channelid;
        _collector.AddChannel(channelId,this);
        SubscribeUser(creator);
        // bad practice but this is specifically for testing purposes
        return channelId;
    }
    public void Destroy(string id) {
        _collector.RemoveChannel(id);
    }
    public void Destroy() {
        _collector.RemoveChannel(channelId);
    }
    public bool IsUserSubscribed(string userid) {
        foreach(string id in _subed_users) {
            if (id == userid) return true;
        }
        return false;
    }
    public void SubscribeUser(string userid) {
        if(IsUserSubscribed(userid)) {
            return;
        }
        _subed_users.Add(userid);
    }
    public void UnSubscribe(string userid) {
        if (IsUserSubscribed(userid)) {
            _subed_users.Remove(userid);
        }
    }
    [Obsolete(message: "Bad coding on my part", error: false ,DiagnosticId = "BadPayload")]
    public async Task broadcast(MessagePayload chat) {
        foreach(string user in _subed_users) {
            await _cache.SendAsync(new Packet(PacketType.PACKET_MESSAGE,chat),user);
        }
    }
    public async Task broadcast(Packet packet) {
        foreach(string user in _subed_users) {
            await _cache.SendAsync(packet,user);
        }
    }
    public async Task broadcastChannelId(string userid) {
        var packet = new Packet(PacketType.PACKET_CREATE_SERVER,new AuthorComponent{ authorId = channelId});
        await _cache.SendAsync(packet,userid);
    }
}
public class HubCollector {
    private ConcurrentDictionary<string,ChatHub> channels = new ();
    public string GetId(ChatHub hub) {
        return channels.FirstOrDefault(p => p.Value == hub).Key;
    }
    public ChatHub GetChatHub(string key) {
        return channels.FirstOrDefault(p => p.Key == key).Value;
    }
    public bool AddChannel(string id, ChatHub hub) {
        return channels.TryAdd(id,hub);
    }
    public bool RemoveChannel(string id) {
        return channels.TryRemove(id,out _);
    }
}