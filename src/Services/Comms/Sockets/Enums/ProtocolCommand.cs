namespace Services.Comms.Sockets
{
    public enum ProtocolCommand : byte
    {
        Unknown = 0x00,
        NACK = 0x01,
        ACK = 0x02,
        Validate = 0x03,
        ClientConnected = 0x04,
        ClientDisconnected = 0x05,
        HeartBeat = 0x06,
        ClientList,
        Custom = 0xFF
    }
}
