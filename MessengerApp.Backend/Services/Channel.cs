namespace MessengerApp.Backend.Services;
public class Channel (ILogger<Channel> logger) {
    // SubscribedNamesHash should be faster for individual look up
    // SubscribedNamesList should be faster for message brodcasts (though reshuffles may take awhile)
    // takes up alot of memomry probably though, but speeds should hopefully be worth it.
    // using a string since it should take the least amount of data and makes a good test
    private readonly HashSet<string> SubscribedNamesHash = new();
    private readonly List<string> SubscribedNamesList = new();
    public void Subscribe(string name) {
        SubscribedNamesHash.Add(name);
        SubscribedNamesList.Add(name);
    }
    public void UnSubscribe(string name) {
        SubscribedNamesHash.Remove(name);
        SubscribedNamesList.Remove(name);
    }
    public async Task Brodcast() {
        
    }
}