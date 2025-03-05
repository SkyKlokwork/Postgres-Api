using Klokwork.ChatApp.DataSources.Client;

namespace Klokwork.ChatApp.DataSources.RequestHandler;
/* 
// Can pass in any packet currently.
// Not Ideal, this lets you have differing types with payloads
*/
public interface IRequestHandler {
    public Task Handle(Packet packet);
}