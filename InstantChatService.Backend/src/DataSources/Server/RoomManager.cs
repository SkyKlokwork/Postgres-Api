using System.Collections.Concurrent;
using Klokwork.ChatApp.DataSources.Client;
using Microsoft.IdentityModel.Tokens;

namespace Klokwork.ChatApp.DataSources.Server;
/* probably a better way to handle inversion of control here but can't think of one at */
/* any more decoupling I can do? */
public class RoomManager(ClientWebSocketCache cache) {
    private readonly ClientWebSocketCache _cache = cache;
    private ConcurrentDictionary<string,Room> _rooms = new ();
    public Room GetRoom(string roomid) {
        return _rooms.TryGetValue(roomid,out Room? room) ? room : null!;   
    }
    // TODO: Figure out disposables
    public Room? CreateRoom(string roomName, string userId) {
        var roomid = UniqueId.CreateUniqueId();
        var temp = new Room(roomName,roomid,userId);
        _rooms.TryAdd(roomid,temp);
        return temp;
    }
    // TODO: add permissions checks
    // would be kinda awkward if just anyone can delet a room
    public void DestroyRoom(string roomid) {
        if(!_rooms.ContainsKey(roomid)) {throw new KeyNotFoundException(nameof(roomid));}
        _rooms.TryRemove(roomid, out _);
    }
    // this will probably need to be threaded
    // also is there a better way of doing this? 
    // feels like it should be the responsibility of the room to broadcast, not the manager
    // at the same time, User probably doesn't need to know about the cache of online users, just those who are subscribed
    // let the cahce sort out if they are online or not.
    public async Task BroadcastToRoomAsync(string roomid, Packet packet) {
        var users = GetRoom(roomid).GetSubscribedUsers();
        foreach (var user in users) {
            await _cache.SendAsync(packet, user);
        }
    }
}