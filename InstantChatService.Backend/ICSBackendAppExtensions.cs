using Klokwork.ChatApp.DataSources.Client;

public static class ICSBackendAppExtensions {
    public static IApplicationBuilder UseClientManager(this IApplicationBuilder app) {
        return app.UseMiddleware<ClientSocketMiddleware>();
    }
}