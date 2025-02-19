using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MessengerApp.Backend.TCP;
public class PackageData {
    // Will likely revamp this class if I decide to add message attachment (pictures)
    // otherwise the client/server will have to handle two packets
    [JsonPropertyName("TYPE")]
    private readonly PackageType _dataType;
    [JsonPropertyName("DATA")]
    private readonly JsonElement _buffer;
    // might make constructors that take specific Classes
    public PackageType getType() {
        return _dataType;
    }
    public JsonElement GetJson() {
        return _buffer;
    }
    public PackageData(PackageType bufferEnum) {
        _dataType = bufferEnum;
        _buffer = new JsonElement();
    }
    public PackageData(PackageType bufferEnum, JsonElement element) {
        _dataType = bufferEnum;
        _buffer = element;
    }
    public void encode() {

    }
    public static JsonElement ToJsonElement(object obj) {
        return JsonSerializer.SerializeToElement(obj);
    }
    // TODO: need to write some encryption logic to keep data relatively obfusticated
    public ArraySegment<byte> ToBuffer() {
        JsonObject json = new JsonObject();
        json.Add("TYPE",(byte)_dataType);
        json.Add("DATA",_buffer.ToString());
        var convertedBytes = JsonSerializer.SerializeToUtf8Bytes(json);
        return new ArraySegment<byte>(convertedBytes);
    }
    public string ToJson() {
        var json = new JsonObject();
        json.Add("TYPE",(byte)_dataType);
        json.Add("DATA",_buffer.ToString());
        var convertedJson = JsonSerializer.Serialize(json);
        return convertedJson;
    }
    public static string FromBufferToString(ArraySegment<byte> buffer) {
        var rawJson = Encoding.UTF8.GetString(buffer);
        return rawJson;
    }
    public static PackageData FromJson(string rawJson) {
        var json = string.IsNullOrEmpty(rawJson) ? new JsonObject() : JsonNode.Parse(rawJson)!.AsObject();
        var output = (json.ContainsKey("TYPE") && json.ContainsKey("DATA"))
            ? new PackageData(
                (PackageType)json["TYPE"]!.GetValue<int>(),
                json["DATA"]!.GetValue<JsonElement>()
            )
            : throw new JsonException("Buffer Data Is Malformed!");
        return output;
    }
    public static PackageData FromBuffer(ArraySegment<byte> buffer) {
        var rawJson = Encoding.UTF8.GetString(buffer);
        var json = string.IsNullOrEmpty(rawJson) ? new JsonObject() : JsonNode.Parse(rawJson)!.AsObject();
        var output = (json.ContainsKey("TYPE") && json.ContainsKey("DATA")) 
            ? new PackageData(
                (PackageType)json["TYPE"]!.GetValue<int>(),
                json["DATA"]!.GetValue<JsonElement>()
            )
            : throw new JsonException("Buffer Info Malformed");
        return output;
    }
}