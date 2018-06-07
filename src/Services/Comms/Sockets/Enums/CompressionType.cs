namespace Services.Comms.Sockets
{
    public enum CompressionType : byte
    {
        Uncompressed = 0x00,
        LZ4 = 0x01
    }
}
