using System.IdentityModel.Tokens.Jwt;

namespace MessengerApp.Backend.Models;
public record Message(
    string id,
    string content,
    // attachment in the future
    string channel_id,
    string author_id,
    bool edited = false
    );
public record Session(string userId) {
    public required string sessionId;
    public required JwtSecurityToken token;
};
