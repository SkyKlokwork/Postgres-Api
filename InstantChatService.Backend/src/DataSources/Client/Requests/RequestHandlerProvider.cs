using Klokwork.ChatApp.DataSources.Client;

namespace Klokwork.ChatApp.DataSources.RequestHandler;
public class RequestHandlerProvider {
    private Dictionary<PacketType,IRequestHandler> _provider = new () {
        {PacketType.PACKET_MESSAGE,new TextHandler()}
        };
    // dumb doo doo code. I'm not feeling great but want something
    // forcing them to add the handler themselves and assign its associated packet type feels somewhat wrong
    // can also lead to annoying issues like them putting the wrong handler in
    // probably better to figure out a way for the packettype to be assigned to the handler on initialization (maybe somehow using generic types?)
    public IRequestHandler? GetRequestHandler(Packet packet) {
        var task = _provider.ContainsKey(packet.Type) ? _provider.GetValueOrDefault(packet.Type) : throw new KeyNotFoundException(nameof(packet.Type));
        task!.Handle(packet);
        return task;
    }
}