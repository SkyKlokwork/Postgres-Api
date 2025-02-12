namespace MessengerApp.Backend.TCP;
public enum BufferDataEnum : byte {

    BUFFER_TYPE_DEFAULT = 0x50,
    BUFFER_TYPE_CHAT = 0x21,
    BUFFER_TYPE_PING = 0x4b,
    BUFFER_TYPE_PONG = 0x4c
}