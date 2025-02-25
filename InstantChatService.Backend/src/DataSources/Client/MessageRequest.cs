

namespace Klokwork.ChatApp.DataSources.Client;
public class MessageRequest : IRequestHandler
{
    public Task HandleRequestAsync(TextChat chat)
    {
        Console.WriteLine(chat.message);
        return Task.CompletedTask;
    }
}