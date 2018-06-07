using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Comms.Sockets
{
    public class Message
    {
        #region Variables
        byte command;

        byte subCommand;

        CompressionType compression;

        byte reserved;
        byte[] body;
        #endregion

        #region Constructors
        public Message()
        {
            command = 0x0;
            subCommand = 0x0;
            reserved = 0x0;
            body = new byte[0];
            compression = CompressionType.Uncompressed;
        }

        public Message(ProtocolCommand Command, CompressionType Compression)
            : this(Command, 0x00, 0x00, Compression, null)
        {
        }
        public Message(ProtocolCommand Command, byte SubCommand, byte Reserved, CompressionType Compression, params object[] Body)
            : this()
        {
            this.Command = Command;
            subCommand = SubCommand;
            body = SetBody(Body);
            compression = Compression;
            reserved = Reserved;
        }


        #endregion

        public void SetInnerbody(byte[] innerBody)
        {
            body = innerBody;
        }
        private byte[] SetBody(object[] body)
        {
            if (body == null || body.Length < 1)
                return new byte[0];
            else
                return Protocol.Encode(body);
        }
        private object[] GetBody(byte[] body)
        {
            if (body == null || body.Length < 1)
                return new object[] { null };
            object[] retval = Protocol.Decode(body);
            return retval;
        }
        #region Properties
        /// <summary>
        /// Message Command
        /// </summary>
        public ProtocolCommand Command { get { return (ProtocolCommand)command; } set { command = (byte)value; } }
        /// <summary>
        /// Message Sub-command
        /// </summary>
        public byte SubCommand { get { return subCommand; } set { subCommand = value; } }
        /// <summary>
        /// IsCompressed
        /// </summary>
        public byte Reserved { get { return reserved; } set { reserved = value; } }
        /// <summary>
        /// Message body
        /// </summary>
        public byte[] InnerBody { get { return body; } }

        public object[] FormatedBody { get { return GetBody(body); ; } set { body = SetBody(value); } }

        /// <summary>
        /// Compression mode
        /// </summary>        
        public CompressionType Compression { get { return compression; } set { compression = value; } }

        #endregion
        
    }
}
