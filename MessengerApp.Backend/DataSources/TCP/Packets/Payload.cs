using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MessengerApp.Backend.Models;
using MessengerApp.Backend.TCP.Packets.Enum;
using Microsoft.IdentityModel.Tokens;

namespace MessengerApp.Backend.TCP;
public sealed class Payload {
    private readonly PayloadHeader _rawContent;
    private string _payload = null!;
    public Payload(PayloadHeader content) {
        _rawContent = content;
    }
    public Payload(JsonObject json) {
        _rawContent = JsonSerializer.Deserialize<PayloadHeader>(json) ?? new PayloadHeader();
    }

    public string GetPayload() {
        ToJsonString();
        return _payload;
    }
    public void ToJsonString() {
        _payload = JsonSerializer.Serialize(_rawContent);
    }
    public JsonObject ToJson() {
        ToJsonString();
        // this is probably bad but its 1am
        // this lets me directly convert in constructor saving me a little sleep
        JsonObject? obj = JsonObject.Parse(_payload) as JsonObject;

        return obj!;
    }
    public static Payload FromJson(JsonObject raw) {
        return new Payload(raw);
    }
}