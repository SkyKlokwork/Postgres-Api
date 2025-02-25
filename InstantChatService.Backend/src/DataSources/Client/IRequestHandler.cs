namespace Klokwork.ChatApp.DataSources.Client;
public interface IRequestHandler {
    Task HandleRequestAsync(TextChat chat);
}