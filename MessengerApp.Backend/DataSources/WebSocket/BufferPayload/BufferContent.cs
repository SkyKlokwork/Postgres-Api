
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using MessengerApp.Backend.Models;
using NetTopologySuite.Operation.Buffer;

namespace MessengerApp.Backend.TCP;
public class BufferContent {
    // Will likely revamp this class if I decide to add message attachment (pictures)
    // otherwise the client/server will have to handle two packets
    [JsonPropertyName("TYPE")]
    private readonly BufferDataEnum _dataType;
    [JsonPropertyName("DATA")]
    private readonly JsonElement _buffer;
    // might make constructors that take specific Classes
    public BufferDataEnum getType() {
        return _dataType;
    }
    public JsonElement GetJson() {
        return _buffer;
    }
    public BufferContent(BufferDataEnum bufferEnum, JsonElement element) {
        _dataType = bufferEnum;
        _buffer = element;
    }
    public void encode() {

    }
    // TODO: need to write some encryption logic to keep data relatively obfusticated
    public ArraySegment<byte> ToBuffer() {
        JsonObject json = new JsonObject();
        json.Add("TYPE",(byte)_dataType);
        json.Add("DATA",_buffer.ToString());
        var convertedBytes = JsonSerializer.SerializeToUtf8Bytes(json);
        return new ArraySegment<byte>(convertedBytes);
    }
    public static string FromBufferToString(ArraySegment<byte> buffer) {
        var rawJson = Encoding.UTF8.GetString(buffer);
        return rawJson;
    }
    public static BufferContent FromBuffer(ArraySegment<byte> buffer) {
        var rawJson = Encoding.UTF8.GetString(buffer);
        var json = string.IsNullOrEmpty(rawJson) ? new JsonObject() : JsonNode.Parse(rawJson)!.AsObject();
        var output = (json.ContainsKey("TYPE") && json.ContainsKey("DATA")) 
            ? new BufferContent(
                (BufferDataEnum)json["TYPE"]!.GetValue<int>(),
                json["DATA"]!.GetValue<JsonElement>()
            )
            : throw new JsonException("Buffer Info Malformed");
        return output;
    }
}