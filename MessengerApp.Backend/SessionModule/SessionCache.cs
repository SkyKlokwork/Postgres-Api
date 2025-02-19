using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using MessengerApp.Backend.TCP;
using Microsoft.IdentityModel.Tokens;

namespace MessengerApp.Backend.SessionModule;
public class SessionCache {
    private ConcurrentDictionary<string,Session> _trackedSessions = new();
    private ConcurrentDictionary<string,WebSocket> _connections = new ();
    [Experimental("SessionCacheExperimental")]
    public Task OpenSession(HttpContext context) {
        string sessionId = UniqueId.CreateUniqueId();
        _trackedSessions.TryAdd(sessionId,new Session(context));
        return Task.CompletedTask;
    }
    public Task AddSocket(string name, WebSocket socket) {
        _connections.TryAdd(name,socket);
        return Task.CompletedTask;
    }
    public Task RemoveUser(string name) {
        _connections.TryRemove(name,out _);
        return Task.CompletedTask;
    }
}