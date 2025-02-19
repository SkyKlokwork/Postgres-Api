using MessengerApp.Backend.Models;

namespace MessengerApp.Backend.Channels;
public interface IChannel {
    public event EventHandler<ChannelBroadCastArgs> BroadcastMessage;
}

public class ChannelBroadCastArgs : EventArgs {
    public Message broadcastedMessage {get; set;} = null!;
}