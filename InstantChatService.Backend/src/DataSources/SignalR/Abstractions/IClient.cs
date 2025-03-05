namespace Klokwork.ChatApp.DataSources.SignalR;
public interface IClient {
    Task ReceiveMessage(string user, string message);
    Task SendMessage(string user, string message);
}