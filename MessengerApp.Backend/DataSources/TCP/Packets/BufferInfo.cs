using MessengerApp.Backend.TCP.Packets.Enum;

namespace MessengerApp.Backend.TCP;

public class BufferInfo {
    int _headerEnum;
    public BufferInfo(BufferDataEnum num) {
        _headerEnum = (byte) num;
    }
}