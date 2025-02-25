using System.Text.Json.Nodes;
using Microsoft.IdentityModel.Tokens;

namespace Klokwork.ChatApp.DataSources.Client;
public record ValidMessage {
}
public record TextChat {
    public string authorId {get; set;} = UniqueId.CreateRandomId();
    public string channelId {get; set;} = UniqueId.CreateRandomId();
    public string serverId {get; set;} = UniqueId.CreateRandomId();
    public string messageId {get; set;} = UniqueId.CreateRandomId();
    public required string message {get; set;}
    public bool isEdited{get; set;} = false;
}