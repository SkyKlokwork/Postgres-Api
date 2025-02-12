using System.Collections.Concurrent;
using System.Net.WebSockets;
using MessengerApp.Backend.TCP;

namespace MessengerApp.Backend.SessionModule;
public class SessionCache {
    // Stores information related to connected clients
    private ConcurrentDictionary<string,Session> connections = new ();
    
}