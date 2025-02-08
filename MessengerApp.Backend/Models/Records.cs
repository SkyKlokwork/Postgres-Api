using System.IdentityModel.Tokens.Jwt;
using MessengerApp.Backend.TCP.Packets.Enum;

namespace MessengerApp.Backend.Models;
public record PayloadHeader(
    PacketTypeEnum header = PacketTypeEnum.PACKET_TYPE_PACKET
);
public record Message(
    string Id,
    string Content,
    // attachment in the future
    string Channel_id,
    string Author_id,
    bool Edited = false
    ) : PayloadHeader(PacketTypeEnum.PACKET_TYPE_CHAT); // Acts somewhat as Contract. Also abstracts header
public record Session(string UserId) {
    public required string sessionId;
    public required JwtSecurityToken token;
};
