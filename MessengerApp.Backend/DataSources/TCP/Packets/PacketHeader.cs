using MessengerApp.Backend.TCP.Packets.Enum;

namespace MessengerApp.Backend.TCP;

public class PacketHeader {
    int _headerEnum;
    public PacketHeader(PacketTypeEnum num) {
        _headerEnum = (byte) num;
    }
}