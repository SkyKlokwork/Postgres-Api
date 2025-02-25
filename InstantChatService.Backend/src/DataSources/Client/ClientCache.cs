using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Options;

namespace Klokwork.ChatApp.DataSources.Client;
public class ClientCache {
    public ConcurrentDictionary<string,WebSocket> _online = new ();
    public ClientSocketOptions Options {get; private set;}
    public ClientCache(IOptions<ClientSocketOptions> options) {
        if (options == null) throw new ArgumentNullException(nameof(options));
        Options = options.Value;
    }
    private byte[] Encode<Tkey>(Tkey obj) {
        return JsonSerializer.SerializeToUtf8Bytes<Tkey>(obj);
    }
    public static async Task SendAsync(byte[] messsage, WebSocket websocket) {
        if(messsage == null || websocket == null || websocket.State != WebSocketState.Open) return;
        await websocket.SendAsync(new ArraySegment<byte>(messsage),WebSocketMessageType.Text,true,CancellationToken.None);
    }
    private static async Task CloseWebSocketAsync(WebSocket websocket) {
        if (websocket.State != WebSocketState.Closed && websocket.State != WebSocketState.Aborted)
            await websocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure," ", default);
    }
    private static async Task ForceCloseSocketAsync(WebSocket websocket,WebSocketCloseStatus status,string reason) {
        if (websocket.State != WebSocketState.Closed && websocket.State != WebSocketState.Aborted)
            await websocket.CloseAsync(status,reason,default);
    }
    public WebSocket GetWebSocket(string key) {
        if(!_online.ContainsKey(key)) throw new KeyNotFoundException(nameof(key));
        var output = _online.FirstOrDefault(k => k.Key == key).Value;
        return output;
    }
    public void Add(string key, WebSocket websocket) {
        ArgumentNullException.ThrowIfNull(websocket);
        // this will probably be the code that handles multiple connections in the future
        var test = _online.TryAdd(key,websocket) ? true : false;
    }
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
}