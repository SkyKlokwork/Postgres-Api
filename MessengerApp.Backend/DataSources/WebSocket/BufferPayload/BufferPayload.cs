using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MessengerApp.Backend.Models;

namespace MessengerApp.Backend.TCP;
public class BufferPayload {
    private readonly BufferDataEnum _bufferType;
    private string _bufferCache;
    private JsonObject _jsonObject =  new JsonObject();

    public BufferDataEnum GetBufferEnum() {return _bufferType;}
    public string GetBufferContent() {return _bufferCache;}
    public JsonObject GetJsonObject() {return _jsonObject;}

    public BufferPayload() {
        _bufferType = BufferDataEnum.BUFFER_TYPE_DEFAULT;
        _bufferCache = JsonSerializer.Serialize(new JsonObject());
    }
    public BufferPayload(BufferDataEnum type) {
        _bufferType = type;
    }
    public BufferPayload(Message message) {
        _bufferType = BufferDataEnum.BUFFER_TYPE_CHAT;
        _bufferCache = JsonSerializer.Serialize(message);
    }
    public ArraySegment<byte> ToBuffer() {
        _jsonObject.Add("TYPE",(byte)_bufferType);
        if(_bufferCache != null) {
            _jsonObject.Add("CONTENT",_bufferCache);
        }
        var intermediary = JsonSerializer.SerializeToUtf8Bytes(_jsonObject);
        return new ArraySegment<byte>(intermediary);
    }
    public static BufferPayload FromBuffer(ArraySegment<byte> buffer) {
        var json = Encoding.UTF8.GetString(buffer);
        JsonObject jsonObj = JsonObject.Parse(json)!.AsObject();
        if((BufferDataEnum)jsonObj["TYPE"]!.GetValue<int>() == BufferDataEnum.BUFFER_TYPE_CHAT) {
            var message = JsonSerializer.Deserialize<Message>(jsonObj["CONTENT"]!.ToString());
            return new BufferPayload(message!);
        }
        return new BufferPayload();
    }
}