using System.Text;
using System.Text.Json;
using MessengerApp.Backend.Models;
using MessengerApp.Backend.TCP.Packets.Enum;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite.Shape.Fractal;

namespace MessengerApp.Backend.TCP.Packets;
public class Chat : Packet
{
    private Message _message;
    public Chat(Message message) {
        header = PacketTypeEnum.PACKET_TYPE_CHAT;
    }
    public void Encode() {
        payload = JsonSerializer.Serialize(_message);
    }

}