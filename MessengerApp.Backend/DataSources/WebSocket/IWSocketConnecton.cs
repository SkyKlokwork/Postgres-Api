using MessengerApp.Backend.Models;

namespace MessengerApp.Backend.TCP;
public interface IWSocketConnecton {
    public Task Close(string reason);
    public Task Send(Message message);
    public Task Listner();
}