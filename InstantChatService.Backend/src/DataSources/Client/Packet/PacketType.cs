namespace Klokwork.ChatApp.DataSources.Client;
public enum PacketType : byte {
    // TODO: Review Hexadecimal values to make sure they observe a standard (assuming there is one, if not make one, dummy!)
    PACKET_DEFAULT = 0x50,
    PACKET_MESSAGE = 0x21,
    PACKET_IDENTITY = 0x80,
    PACKET_ROOM_DATA = 0x45
}