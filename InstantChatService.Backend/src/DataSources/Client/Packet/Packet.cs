using System.Text.Json;
using System.Text.Json.Nodes;

namespace Klokwork.ChatApp.DataSources.Client;
public class Packet {
    public Byte Type {get; set;}
    // object is pretty generic
    // will likely run into issues converting back from the data stream
    public JsonElement Payload {get; set;}
    public Packet(PacketType type, object payload) {
        Type = (byte) type;
        Payload = JsonSerializer.SerializeToElement(payload);
    }
    public Packet(PacketType type, JsonElement payload) {
        Type = (byte) type;
        Payload = payload;
    }
    public JsonObject ToJson() => JsonSerializer.SerializeToNode<Packet>(this)!.AsObject();

    public static Packet ToPacket(string message) {
        var json = string.IsNullOrEmpty(message) ? 
            new JsonObject().AsObject() : 
            JsonNode.Parse(message)!.AsObject();
        var output = (json.ContainsKey("Type") && json.ContainsKey("Payload")) ?
            new Packet( 
                (PacketType)json["Type"]!.GetValue<byte>(),
                json["Payload"]!.Deserialize<JsonElement>()
            ) :
            throw new JsonException("Malformed Packet JSON");
        return output;
    }
}