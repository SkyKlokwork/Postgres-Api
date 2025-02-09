using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MessengerApp.Backend.Models;
using MessengerApp.Backend.TCP.Packets.Enum;

namespace MessengerApp.Backend.TCP;
public sealed class BufferPayload : IDisposable{
    private readonly BufferDataEnum _bufferType;
    private string _bufferContent;
    private JsonObject _jsonObject =  new JsonObject();

    public BufferDataEnum GetBufferEnum() {return _bufferType;}
    public string GetBufferContent() {return _bufferContent;}
    public JsonObject GetJsonObject() {return _jsonObject;}

    public BufferPayload() {
        _bufferType = BufferDataEnum.BUFFER_TYPE_DEFAULT;
        _bufferContent = JsonSerializer.Serialize(new JsonObject());
    }
    public BufferPayload(Message message) {
        _bufferType = BufferDataEnum.BUFFER_TYPE_CHAT;
        _bufferContent = JsonSerializer.Serialize(message);
    }
    public ArraySegment<byte> ToBuffer() {
        _jsonObject.Add("TYPE",(byte)_bufferType);
        _jsonObject.Add("CONTENT",_bufferContent);
        var intermediary = JsonSerializer.SerializeToUtf8Bytes(_jsonObject);
        return new ArraySegment<byte>(intermediary);
    }
    public static BufferPayload FromBuffer(ArraySegment<byte> buffer) {
        var json = Encoding.UTF8.GetString(buffer);
        JsonObject jsonObj = JsonObject.Parse(json)!.AsObject();
        if((BufferDataEnum)jsonObj["TYPE"]!.GetValue<int>() == BufferDataEnum.BUFFER_TYPE_CHAT) {
            var message = JsonSerializer.Deserialize<Message>(jsonObj["CONTENT"]!.ToString());
            return new BufferPayload(message);
        }
        return new BufferPayload();
    }
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}