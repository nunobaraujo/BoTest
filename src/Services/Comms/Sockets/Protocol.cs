using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Services.Comms.Sockets
{
    public static class Protocol
    {
        public const long ClientSignature = 506337203688975807;
        internal const long ServerSignature = 8122857854686597437;

        #region Constants
        public const int DefaultPort = 9865;

        /// <summary>
        /// Start Of Header
        /// </summary>
        internal const byte SOH = 0x01;
        /// <summary>
        /// Start of Text
        /// </summary>
        internal const byte STX = 0x02;
        /// <summary>
        /// End of Text
        /// </summary>
        internal const byte ETX = 0x03;
        /// <summary>
        /// End of transmission
        /// </summary>
        internal const byte EOT = 0x04;
        /// <summary>
        /// Carriage Return
        /// </summary>
        internal const byte CR = 0x0D;
        /// <summary>
        /// Line Feed
        /// </summary>
        internal const byte LF = 0x0A;
        /// <summary>
        /// New Page (Parameter separator)
        /// </summary>
        internal const byte NP = 0x0C;
        #endregion

        

        #region Encoding

        public static byte[] EncodeMessageBytes(Message Message)
        {
            try
            {
                byte[] MsgBody;
                if (Message.InnerBody.Length > 0)
                {
                    if (Message.InnerBody.Length > 1024 * 1024)
                        Message.Compression = CompressionType.LZ4;
                    else
                        Message.Compression = CompressionType.Uncompressed;


                    switch (Message.Compression)
                    {
                        default:
                        case CompressionType.Uncompressed:
                            MsgBody = Message.InnerBody;
                            break;
                        case CompressionType.LZ4:
                            MsgBody = CompressLZ4(Message.InnerBody);
                            break;

                    }
                }
                else
                    MsgBody = new byte[0];



                byte[] BodySize = BitConverter.GetBytes(MsgBody.Length);
                List<byte> MSG = new List<byte>();
                MSG.Add(SOH);                       // Start of message and header
                MSG.Add(0x07);                      // MessageType
                MSG.Add((byte)Message.Command);     // Command
                MSG.Add(Message.SubCommand);        // Sub-command
                MSG.Add((byte)Message.Compression); // Compression Used
                MSG.Add(Message.Reserved);          // Reserved Byte
                MSG.AddRange(BodySize);             // Body Size
                MSG.Add(STX);                       // Start of body (parameters)            
                MSG.AddRange(MsgBody);              // Body
                MSG.Add(ETX);                       // End of body
                return MSG.ToArray();

            }
            catch
            {
                return new byte[0];
            }
        }
        public static Message DecodeMessageBytes(byte[] ReceivedBytes)
        {
            Message retval = new Message();

            if (!IsMessageComplete(ReceivedBytes))
                throw new FormatException("Unknown message");

            // Get Message Header
            byte check = ReceivedBytes[1];
            if (check != 0x07)
                throw new FormatException("Unknown message");

            retval.Command = (ProtocolCommand)ReceivedBytes[2];
            retval.SubCommand = ReceivedBytes[3];
            retval.Compression = (CompressionType)ReceivedBytes[4];
            retval.Reserved = ReceivedBytes[5];

            List<object> pars = new List<object>();
            List<byte> bpar = new List<byte>();

            //Byte 6, 7, 8, 9 - traz o tamanho do body
            int BodySize = BitConverter.ToInt32(ReceivedBytes, 6);

            if (BodySize > 0)
            {

                byte[] body = new byte[BodySize];
                Array.Copy(ReceivedBytes, 11, body, 0, body.Length);


                switch (retval.Compression)
                {
                    case CompressionType.Uncompressed:
                        retval.SetInnerbody(body);
                        break;
                    case CompressionType.LZ4:
                        retval.SetInnerbody(DecompressLZ4(body));
                        break;
                    default:
                        break;
                }

            }
            else
                retval.SetInnerbody(new byte[0]);

            //Console.WriteLine("Receiving Body Size Compressed: [{0}] bytes ", Message.Body.Length);

            return retval;
        }

        public static byte[] Encode<T>(T parameter)
        {
            if (parameter == null)
                return new byte[0];

            return Encode(new List<byte[]>() { ModelSerializer.Serialize<T>(parameter) });
        }
        private static byte[] Encode(List<byte[]> parameters)
        {
            int l = 0;
            for (int i = 0; i < parameters.Count; i++)
            {
                //parameters[i] = CommProtocol.Protocol.CompressLZ4(parameters[i]);
                l += parameters[i].Length;
                //byte[] s = CommProtocol.Protocol.CompressLZ4(item);
            }

            byte[] bodyBuff = new byte[parameters.Count * 4 + l];


            try
            {
                // Build the body
                int bodyctr = -1;
                foreach (byte[] barray in parameters)
                {
                    foreach (var item in barray)        // Parameter
                    {
                        bodyBuff[++bodyctr] = item;
                    }
                    bodyBuff[++bodyctr] = CR;           // Parameter separator: CR+NP+NP+LF
                    bodyBuff[++bodyctr] = NP;
                    bodyBuff[++bodyctr] = NP;
                    bodyBuff[++bodyctr] = LF;
                }
                // Copy the body to a sized byte array
                byte[] BodyMsg = new byte[bodyctr + 1];
                Array.Copy(bodyBuff, BodyMsg, bodyctr + 1);
                return BodyMsg;
            }
            catch
            {
                throw;
            }
            finally
            {
                bodyBuff = null;
            }

        }

        
        internal static List<byte[]> Decode(byte[] body)
        {
            //byte[] body = CommProtocol.Protocol.DecompressLZ4(bodyCompressed);

            List<byte[]> pars = new List<byte[]>();
            List<byte> bpar = new List<byte>();

            for (int i = 0; i < body.Length; i++)
            {
                if (body[i] == CR && body[i + 1] == NP && body[i + 2] == NP && body[i + 3] == LF)   // End of Parameter
                {
                    //i = i + 2;

                    try
                    {
                        pars.Add(bpar.ToArray());
                        //pars.Add(CommProtocol.Protocol.DecompressLZ4(bpar.ToArray()));
                    }
                    catch
                    {
                        return null;
                    }
                    i = i + 3;
                    bpar = new List<byte>();
                }
                else
                {
                    bpar.Add(body[i]);
                }
            }
            return pars;
        }

        #region Serialization

       
        #endregion

        public static bool IsMessageComplete(byte[] ReceivedBytes)
        {
            // Size Validation
            if (ReceivedBytes.Length < 11)
                return false;

            // Header validation
            if (ReceivedBytes[0] != SOH || ReceivedBytes[10] != STX)
                throw new System.Net.ProtocolViolationException();

            // End Body Validation
            if (ReceivedBytes[ReceivedBytes.Length - 1] != ETX)
                return false;

            int BodySize = BitConverter.ToInt32(ReceivedBytes, 6);

            // Header + Body + End Body       
            int MsgSize = 11 + BodySize + 1;

            if (MsgSize == ReceivedBytes.Length)
                return true;
            else if (ReceivedBytes.Length > MsgSize)
            {
                byte[] trimmed = new byte[MsgSize];
                Array.Copy(ReceivedBytes, trimmed, MsgSize);
                ReceivedBytes = trimmed;
                return true;
            }
            else
                return false;
        }

        #endregion

        public static Message ValidateClient(Guid TerminalId)
        {
            Message retval = new Message(ProtocolCommand.Validate, 0xFF, 0x07,
                CompressionType.Uncompressed, 
                new List<byte[]>
                {
                    ModelSerializer.Serialize(ClientSignature),
                    ModelSerializer.Serialize(TerminalId)
                });
            return retval;
        }

        private static byte[] CompressLZ4(byte[] UncompressedData)
        {
            List<byte> retval = new List<byte>();
            byte[] len = BitConverter.GetBytes(UncompressedData.Length);
            retval.AddRange(len);

            byte[] msg = Compressor.CompressBytesLZ4(UncompressedData, 0, UncompressedData.Length);
            retval.AddRange(msg);
            return retval.ToArray();
        }
        private static byte[] DecompressLZ4(byte[] CompressedData)
        {
            Int32 size = 0;
            size = BitConverter.ToInt32(CompressedData, 0);
            if (size == 0)
            {
                return new byte[0];
            }
            byte[] retval = Compressor.DecompressBytesLZ4(CompressedData, 4, CompressedData.Length - 4, size);
            return retval;
        }
    }
}
