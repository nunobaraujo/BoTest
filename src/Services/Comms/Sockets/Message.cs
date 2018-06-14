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
            : this(Command, 0x00, 0x00, Compression)
        {
        }
        public Message(ProtocolCommand Command, byte SubCommand, byte Reserved, CompressionType Compression)
            : this()
        {
            this.Command = Command;
            subCommand = SubCommand;
            compression = Compression;
            reserved = Reserved;
        }


        #endregion
               
       
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

        //public object[] FormatedBody { get { return GetBody(body); ; } set { body = SetBody(value); } }
                
        /// <summary>
        /// Compression mode
        /// </summary>        
        public CompressionType Compression { get { return compression; } set { compression = value; } }

        #endregion

        public T GetParameter<T>()
        {
            return GetParameter<T>(0);
        }
        public T GetParameter<T>(int paramIndex)
        {
            var paramList = Protocol.DecodeBody(body);
            return ModelSerializer.Deserialize<T>(paramList[paramIndex]);
        }
        public void AddParameter<T>(T parameter)
        {
            var paramList = Protocol.DecodeBody(body);
            paramList.Add(ModelSerializer.Serialize(parameter));
            body = Protocol.EncodeBody(paramList);
        }

        internal void SetInnerbody(byte[] innerBody)
        {
            body = innerBody;
        }

        public static Message Deserialize(byte[] message)
        {
            return Protocol.DecodeMessageBytes(message);
        }
        public static byte[] Serialize(Message message)
        {
            return Protocol.EncodeMessageBytes(message);
        }
    }
    
}
