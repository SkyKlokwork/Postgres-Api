using System.Threading.Channels;
using Klokwork.ChatApp.DataSources.Client;

namespace Klokwork.ChatApp.DataSources.Server;
public class Room {
    private readonly Channel<Packet> channel;
    private readonly List<string> _subscribedUsers = new ();
    private readonly string channelId;
    private string channelName;
    public Room(string channel_name, string channel_id, string user_id) {
        channelName = channel_name;
        channelId = channel_id;
        Subsribe(user_id);
    }
    public void Subsribe(string userId) {
        lock (_subscribedUsers) {
            _subscribedUsers.Add(userId);
        }
    }
    public void UnSubscribe(string userId) {
        lock (_subscribedUsers) {
            _subscribedUsers.Remove(userId);
        }
    }
    /* Need to decide how user routes to the channel?? */
    /* Likely will just have a method to grab subscribed channels from the database and store on clients end */
    public void Destroy(){}
    // 
    // Get
    // 
    public string GetChannelName() {
        return channelName;
    }
    // 
    // Set
    // 
    public void SetChannelName(string name) {
        channelName = name;
    }
    public List<string> GetSubscribedUsers() {
        lock (_subscribedUsers) {
            return _subscribedUsers;
        }
    }
}