using System.Text.Json.Nodes;
using Microsoft.IdentityModel.Tokens;

namespace Klokwork.ChatApp.DataSources.Client;
public record AuthorComponent {
    public string authorId {get; set;} = UniqueId.CreateRandomId();

}
public record RoutingComponent : AuthorComponent {
    public string channelId {get; set;} = UniqueId.CreateRandomId();
    public string serverId {get; set;} = UniqueId.CreateRandomId();
}
public record MessagePayload : RoutingComponent{
    public string messageId {get; set;} = UniqueId.CreateRandomId();
    public required string message {get; set;}
    public bool isEdited{get; set;} = false;
}
public record ImagePayload : RoutingComponent {
    
}