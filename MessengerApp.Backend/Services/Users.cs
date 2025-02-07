using System.Collections.Concurrent;

namespace MessengerApp.Backend.Services;
public class Users(ILogger<Users> logger) {
    private readonly ConcurrentDictionary<string,ISession> onlineUsers = new();
    private readonly ConcurrentBag<string> storedUsers = new();

    // Temporary
    // Will eventually use a UUID for individuality
    // Will eventually allow USers to have the same display name
    public string? TryLogin(string name, ISession session)
    {
        if (onlineUsers.ContainsKey(name)) {
            return $"{name} taken" ;
        }
        onlineUsers.TryAdd(name, session);
        return null;
    }
    public void Logout(string name) {
        onlineUsers.TryRemove(name, out _);
    }
}