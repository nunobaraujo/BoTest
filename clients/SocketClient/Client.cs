using Contracts.Api;
using Contracts.Clients;
using NBsoft.Logs.Interfaces;
using Services.Comms.Sockets;
using SocketClient.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocketClient
{
    public class Client : NBsoft.Sockets.SocketClient, IRestClient
    {
        const int Timeout = 30 * 1000;
                
        private readonly Guid _socketId;
        private readonly List<byte> _completeMessage;

        private bool _answerReceived;
        private Message _lastAnswer;

        private readonly string _host;
        private readonly int _port;
        private readonly ILogger _log;

        public Client(string host, int port, ILogger log)
        {
            _socketId = Guid.NewGuid();
            _completeMessage = new List<byte>();
            _host = host;
            _port = port;
            _log = log;

            SessionApi = new Session(this);
        }

        public ISessionApi SessionApi { get; }
        public IUserApi UserApi { get; }
        public ICompanyApi CompanyApi { get; }
        public IJobApi JobApi { get; }

        public void Connect()
        {
            if (!IsConnected)
                Connect(_host, _port);
        }
        private void Connect(string host, int port)
        {
            System.Net.IPAddress.TryParse(host, out System.Net.IPAddress current);
            if (current == null)
            {
                System.Net.IPAddress[] addresslist = System.Net.Dns.GetHostAddresses(host);
                foreach (System.Net.IPAddress address in addresslist)
                {
                    if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        current = address;
                        break;
                    }
                }
            }
            if (current == null)
                throw new ApplicationException("Host not found");
            base.Connect(current, port);

            bool IsValid = Validate();
            if (!IsValid)
                throw new ApplicationException("Invalid Protocol");
        }

        private bool Validate()
        {
            Message outgoing = Protocol.ValidateClient(_socketId);
            Message Response = SendMessageToServer(Protocol.EncodeMessageBytes(outgoing));
            if (Response.Command == ProtocolCommand.ACK)
                return true;
            else
                return false;
        }

        private void SendAck()
        {
            Message NACKmsg = new Message(ProtocolCommand.ACK, CompressionType.Uncompressed);
            Send(Protocol.EncodeMessageBytes(NACKmsg));
        }
        private void SendNack()
        {
            Message NACKmsg = new Message(ProtocolCommand.NACK, CompressionType.Uncompressed);
            Send(Protocol.EncodeMessageBytes(NACKmsg));
        }

        public Message SendCustomMessage(SubCommand subCommand, byte CommandOptions, params object[] pars)
        {
            Message outgoing = new Message(ProtocolCommand.Custom, (byte)subCommand, CommandOptions, CompressionType.Uncompressed, pars);
            return SendMessageToServer(Protocol.EncodeMessageBytes(outgoing));
        }
        private Message SendMessageToServer(byte[] msg)
        {
            _answerReceived = false;
            Send(msg);
            int timectr = 0;
            while (!_answerReceived)
            {
                if (timectr > Timeout)
                    throw new TimeoutException("No answer from server");

                System.Threading.Thread.Sleep(100);
                timectr += 100;
            }
            _answerReceived = false;
            return _lastAnswer;
        }

        #region Event Handlers

        protected override void OnDataReceived(NBsoft.Sockets.SockMessageEventArgs e)
        {
            base.OnDataReceived(e);
            _completeMessage.AddRange(e.SockMessage);
            byte[] tmp = _completeMessage.ToArray();

            bool IsComplete;
            try { IsComplete = Protocol.IsMessageComplete(tmp); }
            catch
            {
                IsComplete = false;
                SendNack();
            }

            if (IsComplete)
            {
                _completeMessage.Clear();
                //Console.WriteLine("Client: Received Message Size [{0}] bytes", tmp.Length);
                Message received = Protocol.DecodeMessageBytes(tmp);


                switch (received.Command)
                {
                    // Servidor enviou uma mensagem de protocolo, tratar a mensagem conforme protocolo.                        
                    // Se for ACK ou NACK é uma resposta a um pedido do cliente, definir ultima resposta recebida e 
                    // marcar flag de resposta terminada como verdadeira.
                    case ProtocolCommand.ACK:
                    case ProtocolCommand.NACK:
                        _lastAnswer = received;
                        _answerReceived = true;
                        break;
                    case ProtocolCommand.Unknown:
                        // Servidor enviou uma mensagem desconhecida, ignorar.
                        break;
                    case ProtocolCommand.ClientConnected:
                        string cclient = received.FormatedBody[0].ToString();
                        //OnClientConnected(new HostEventArgs(cclient));
                        break;
                    case ProtocolCommand.ClientDisconnected:
                        string dclient = received.FormatedBody[0].ToString();
                        //OnClientDisconnected(new HostEventArgs(dclient));
                        break;
                    case ProtocolCommand.Custom:
                        // Servidor enviou uma mensagem personalizada
                        // Responder ACK e lançar evento de mensagem recebida.
                        SendAck();
                        //OnServerMessage(new MessageEventArgs(received));
                        break;
                    default:
                        break;
                }
            }

        }
        protected override void OnFileReceived(NBsoft.Sockets.FileEventArgs e)
        {
            base.OnFileReceived(e);
        }
        #endregion
    }
}
