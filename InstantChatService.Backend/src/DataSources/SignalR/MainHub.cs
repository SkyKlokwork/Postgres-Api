using Microsoft.AspNetCore.SignalR;

namespace Klokwork.ChatApp.DataSources.SignalR;
public class MainHub : Hub<IClient>
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.ReceiveMessage(user,message);
    }
}