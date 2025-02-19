namespace MessengerApp.Backend.TCP;
public enum PackageType : byte {

    PACKAGE_TYPE_DEFAULT = 0x50,
    PACKAGE_TYPE_CHAT = 0x21,
    PACKAGE_TYPE_PING = 0x4b,
    PACKAGE_TYPE_PONG = 0x4c
}