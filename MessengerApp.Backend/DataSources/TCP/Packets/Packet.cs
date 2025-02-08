using System.Text.Json;
using System.Text.Json.Nodes;
using MessengerApp.Backend.TCP.Packets.Enum;
using Microsoft.IdentityModel.Tokens;

namespace MessengerApp.Backend.TCP;
public class Packet {
    protected readonly string UUID = UniqueId.CreateRandomId();
    protected PacketTypeEnum header;
    // protected PacketHeader header = new PacketHeader(PacketTypeEnum.PACKET_TYPE_CHAT);
    protected string payload;
    public Packet() {
        header = PacketTypeEnum.PACKET_TYPE_PACKET;
    }
    public virtual void Encode() {
        throw new NotImplementedException("This method is implemented for inherited classes to override");
    }
    public JsonObject ToJson() {
        Encode();
        JsonObject obj = new JsonObject();
        obj.Add("UUID",UUID);
        obj.Add("HEADER",(byte) header);
        obj.Add("PAYLOAD",payload);

        return obj;
    }
    public Packet GetPacketType() {
        return new Packet();
    }
    
}