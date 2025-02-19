using MessengerApp.Backend.Models;

namespace MessengerApp.Backend.Channels;
public class Channel : IChannel {
    private string ChannelID;
    public event EventHandler<ChannelBroadCastArgs> BroadcastMessage;
    protected virtual void OnBroadcase(ChannelBroadCastArgs e) {
        BroadcastMessage.Invoke(this,e);
    }
}