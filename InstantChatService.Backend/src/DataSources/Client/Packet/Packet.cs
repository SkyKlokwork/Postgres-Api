using System.Text.Json;
using System.Text.Json.Nodes;

namespace Klokwork.ChatApp.DataSources.Client;
public class Packet {
    public PacketType Type {get; set;}
    // TODO: clean up
    // object is pretty generic
    // will likely run into issues converting back from the data stream
    public JsonElement Payload {get; set;}
    public Packet(PacketType type, object payload) {
        Type = type;
        Payload = JsonSerializer.SerializeToElement(payload);
    }
    public Packet(PacketType type, JsonElement payload) {
        Type = type;
        Payload = payload;
    }
    public Packet(byte type, object payload) {
        Type = (PacketType)type;
        Payload = JsonSerializer.SerializeToElement(payload);
    }
    public Packet(byte type, JsonElement payload) {
        Type = (PacketType)type;
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