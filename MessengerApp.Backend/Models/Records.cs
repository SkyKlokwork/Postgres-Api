using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using MessengerApp.Backend.TCP.Packets.Enum;

namespace MessengerApp.Backend.Models;
public record Message(
    string Id,
    string Content,
    // attachment in the future
    string Channel_id,
    string Author_id,
    bool Edited = false
);
public record Session(string UserId) {
    public required string sessionId;
    public required JwtSecurityToken token;
};
