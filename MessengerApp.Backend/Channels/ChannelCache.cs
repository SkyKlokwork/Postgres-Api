using System.Collections.Concurrent;

namespace MessengerApp.Backend.Channels;
public class ChannelCache {
    private ConcurrentDictionary<string,Channel> _channels = new();
}