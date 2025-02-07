using MessengerApp.Backend.Models;

namespace MessengerApp.Backend.Services;
public interface ISession {
    Task Close(string reason);
    Task Send(Message message);
}