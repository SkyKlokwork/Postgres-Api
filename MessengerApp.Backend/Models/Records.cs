using System.IdentityModel.Tokens.Jwt;

namespace MessengerApp.Backend.Models;
public record Message(
    string Id,
    string Content,
    // attachment in the future
    string Channel_id,
    string Author_id,
    bool Edited = false
);
public record Ping(
    string Author_id
);
