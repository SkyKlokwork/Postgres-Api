namespace Klokwork.ChatApp.DataSources.Client;
public enum PacketType : byte {
    PACKET_DEFAULT = 0x50,
    PACKET_MESSAGE = 0x21,
    PACKET_CREATE_SERVER = 0x41,
    PACKET_DESTROY_SERVER = 0x43,
    PACKET_SUBSCRIBE = 0x31,
    PACKET_UNSUBSCRIBE = 0x33,
    PACKET_IDENTITY = 0x80
}