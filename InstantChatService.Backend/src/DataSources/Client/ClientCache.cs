using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Klokwork.ChatApp.DataSources.Client;
// using three slashes gives summary
public class ClientCache {
    public ConcurrentDictionary<string,WebSocket> _online = new ();
    public ClientSocketOptions Options {get; private set;}
    public ClientCache(IOptions<ClientSocketOptions> options) {
        if (options == null) throw new ArgumentNullException(nameof(options));
        Options = options.Value;
    }
    // Maybe move these static methods to a new class?
    private static async Task SendAsync(byte[] messsage, WebSocket websocket) {
        if(websocket.State != WebSocketState.Open) return;
        await websocket.SendAsync(new ArraySegment<byte>(messsage),WebSocketMessageType.Text,true,CancellationToken.None);
    }
    private static async Task CloseWebSocketAsync(WebSocket websocket) {
        if (websocket.State != WebSocketState.Closed && websocket.State != WebSocketState.Aborted)
            await websocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure," ", default);
    }
    // likely will need redoing
    private static async Task ForceCloseSocketAsync(WebSocket websocket,WebSocketCloseStatus status,string reason) {
        if (websocket.State != WebSocketState.Closed && websocket.State != WebSocketState.Aborted)
            await websocket.CloseAsync(status,reason,default);
    }
    /*
    Get Methods
    */
    public WebSocket GetWebSocket(string key) {
        if(!_online.ContainsKey(key)) throw new KeyNotFoundException(nameof(key));
        var output = _online.FirstOrDefault(k => k.Key == key).Value;
        return output;
    }
    /**
    Add Methods
    **/
    public void Add(string key, WebSocket websocket) {
        ArgumentNullException.ThrowIfNull(websocket);
        // this will probably be the code that handles multiple connections in the future
        var test = _online.TryAdd(key,websocket) ? true : false;
    }
    /**
    Remove Methods
    **/
    public async Task RemoveAsync(string key, WebSocket webSocket) {
        ArgumentNullException.ThrowIfNull(webSocket);
        if(!_online.ContainsKey(key)) throw new KeyNotFoundException(nameof(key));
        var wasRemoved = _online.TryRemove(key,out _);
        await CloseWebSocketAsync(webSocket);
    }
    // will need refactoring assuming multiple connections
    public async Task RemoveAllAsync() {
        foreach(var keyValue in _online) {
            await RemoveAsync(keyValue.Key,keyValue.Value);
        }
        _online.Clear();
    }
    public async Task ForceRemoveAsync(string key, WebSocket webSocket,string reason) {
        ArgumentNullException.ThrowIfNull(webSocket);
        if(!_online.ContainsKey(key)) throw new KeyNotFoundException(nameof(key));
        _online.TryRemove(key, out _);
        await ForceCloseSocketAsync(webSocket,WebSocketCloseStatus.InternalServerError,reason);
    }
    /* 
    Send Methods
    */
    public async Task SendAsync(Packet packet,WebSocket websocket) {
        if(packet == null || websocket == null) return;
        byte[] messageBuffer = JsonSerializer.SerializeToUtf8Bytes(packet);
        await SendAsync(messageBuffer,websocket);
    }
    public async Task SendAsync(Packet packet,string key) {
        if (packet == null || key == null) return;
        var socket = _online.ContainsKey(key) ? 
            _online.GetValueOrDefault(key) : 
            throw new KeyNotFoundException(nameof(key));
        byte[] messageBuffer = JsonSerializer.SerializeToUtf8Bytes(packet);
        await SendAsync(messageBuffer,socket!);
    }
    
}