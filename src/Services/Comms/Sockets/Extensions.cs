using NBsoft.Sockets;

namespace Services.Comms.Sockets
{
    internal static class Extensions
    {
        public static void SendNack(this ClientConnection CliConn)
        {
            CliConn.Send(new Message(ProtocolCommand.NACK, CompressionType.Uncompressed));
        }
        public static void SendAck(this ClientConnection CliConn)
        {
            CliConn.Send(new Message(ProtocolCommand.ACK, CompressionType.Uncompressed));
        }
        public static void Send(this ClientConnection CliConn, Message message)
        {
            CliConn.Client.Send(message);
        }
        public static void Send(this SocketClient client, Message message)
        {
            client.Send(message.Encode());
        }
        internal static byte[] Encode(this Message msg)
        {
            return Message.Serialize(msg);
        }
    }
}
