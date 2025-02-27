using Klokwork.ChatApp.DataSources.Hubs;

namespace Klokwork.ChatApp.DataSources.Client;
public interface IRequestHandler<T> {
    public Task HandleRequestAsync(HubCollector collector,Packet packet);
}