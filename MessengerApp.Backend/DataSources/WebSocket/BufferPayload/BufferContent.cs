
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
    private readonly BufferDataEnum _dataType;
    private readonly JsonElement _buffer;
    // might make constructors that take specific Classes
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
        json.Add("CONTENT",_buffer.GetString());
        var convertedBytes = JsonSerializer.SerializeToUtf8Bytes(json);
        return new ArraySegment<byte>(convertedBytes);
    }
    public static BufferContent FromBuffer(ArraySegment<byte> buffer) {
        var rawJson = Encoding.UTF8.GetString(buffer);
        var json = JsonObject.Parse(rawJson);
        throw new NotImplementedException();
    }
}