using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;

namespace Klokwork.ChatApp.DataSources.Client;
public class ClientWebSocketCache(IOptions<ClientSocketOptions> options) {
    private readonly ConcurrentDictionary<string,WebSocket> _websocket_cache = new ();
    public ClientSocketOptions options = options.Value ?? throw new ArgumentNullException(nameof(options));
    private async Task CloseSocket(WebSocket socket,WebSocketCloseStatus status, string reason) {
        /* probably don't need this test since it should only been running when open or close sent */
        if (socket.State != WebSocketState.Closed && socket.State != WebSocketState.Aborted)
            await socket.CloseAsync(status, reason, default);
    }
    private static async Task SendAsync(byte[] buffer, WebSocket socket) {
        if(socket.State == WebSocketState.Open) {
            await socket.SendAsync(new ArraySegment<byte>(buffer),WebSocketMessageType.Text,true,CancellationToken.None);
        }
    }
    // 
    // Get
    // 
    public WebSocket GetSocket(string userId) {
        return _websocket_cache.FirstOrDefault(s=>s.Key == userId).Value ??
            throw new KeyNotFoundException(nameof(userId));
    }
    public ConcurrentDictionary<string,WebSocket> GetAllUsers() {
        return _websocket_cache;
    }
    public string GetID(WebSocket socket) {
        return _websocket_cache.FirstOrDefault(s => s.Value == socket).Key ?? 
            throw new KeyNotFoundException("No Key associated with this socket");
    }
    public bool UserOnline(string userid) {
        return _websocket_cache.TryGetValue(userid, out _) ? 
            true : 
            throw new KeyNotFoundException(nameof(userid));
    }
    // 
    // Add
    // 
    public bool Add(string userid, WebSocket socket) {
        if(string.IsNullOrEmpty(userid)) {throw new Exception(nameof(userid));}
        ArgumentNullException.ThrowIfNull(socket);
        return _websocket_cache.TryAdd(userid, socket);
    }
    // 
    // Removes
    // 
    public async Task RemoveAsync(string userid) {
        UserOnline(userid);
        if (!_websocket_cache.TryRemove(userid, out WebSocket? web)) {throw new NullReferenceException(nameof(web));}
        await CloseSocket(web,WebSocketCloseStatus.NormalClosure, "Normal Closure");
    }
    public async Task RemoveAsync(WebSocket socket) {
        string id = GetID(socket);
        if(!_websocket_cache.TryRemove(id,out socket!)) {throw new NullReferenceException(nameof(socket));}
        await CloseSocket(socket,WebSocketCloseStatus.NormalClosure, "Normal Closure");
    }
    public Task RemoveAllAsync() {
        /* probably don't need to lock it but we don't want people connecting while trying to purge the users */
        lock (_websocket_cache) {
            foreach (KeyValuePair<string, WebSocket> pair in _websocket_cache) {
                Task.Run(() => RemoveAsync(pair.Key));
            }
        }
        return Task.CompletedTask;
    }
    // 
    // Send
    // 
    public async Task SendAsync(Packet packet, string userId) {
        if (packet is null && userId is null) {return;}
        WebSocket socket = GetSocket(userId);
        byte[] buffer = JsonSerializer.SerializeToUtf8Bytes(packet);
        await SendAsync(buffer,socket);
    }
    public async Task SendAsync(Packet packet, WebSocket socket) {
        if (packet is null && socket is null) {return;}
        byte[] buffer = JsonSerializer.SerializeToUtf8Bytes(packet);
        await SendAsync(buffer,socket);

    }
    public async Task SendAllAsync(Packet packet) {
        if(packet.Type != PacketType.PACKET_MESSAGE) {throw new ArgumentException("Non message packets shouldn't be sent to everyone");}
        byte[] buffer = JsonSerializer.SerializeToUtf8Bytes(packet);
        foreach (KeyValuePair<string,WebSocket> pair in _websocket_cache) {
            await SendAsync(buffer,pair.Value);
        }
    }
}