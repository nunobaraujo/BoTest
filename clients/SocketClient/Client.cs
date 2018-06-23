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

            SessionApi = new SessionApi(this);
            UserApi = new UserApi(this);
            BusinessApi = new BusinessApi(this);
        }

        public ISessionApi SessionApi { get; }

        public IUserApi UserApi { get; }
        public ICompanyApi CompanyApi { get; }

        public IJobApi JobApi { get; }
        public ICompanyOptionsApi CompanyOptionsApi { get; }
        public ICustomerApi CustomerApi { get; }
        public ICustomerRouteApi CustomerRouteApi { get; }
        public IDocumentTypeApi DocumentTypeApi { get; }

        public IBusinessApi BusinessApi { get; }

        public IDocumentSeriesApi DocumentSeriesApi => throw new NotImplementedException();

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
            var message = SendBytes(Message.Serialize(Protocol.ValidateClient(_socketId)));
            if (message.Command == ProtocolCommand.ACK)
                return true;
            return false;
        }

        private void SendAck()
        {   
            this.SendMessage(new Message(ProtocolCommand.ACK, CompressionType.Uncompressed));
        }
        private void SendNack()
        {
            this.SendMessage(new Message(ProtocolCommand.NACK, CompressionType.Uncompressed));
        }        
        private Message SendBytes(byte[] msg)
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

        public Message SendCustomMessage<T>(SubCommand subCommand, T parameter)
        {
            return SendCustomMessage(subCommand, 0x00, parameter);
        }
        public Message SendCustomMessage<T>(SubCommand subCommand, byte CommandOptions, T parameter)
        {
            var message = new Message(ProtocolCommand.Custom, (byte)subCommand, CommandOptions, CompressionType.Uncompressed);
            message.AddParameter(parameter);
            return SendBytes(Message.Serialize(message));
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
                Message received = Message.Deserialize(tmp);

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
                        string cclient = received.GetParameter<string>();
                        //OnClientConnected(new HostEventArgs(cclient));
                        break;
                    case ProtocolCommand.ClientDisconnected:
                        string dclient = received.GetParameter<string>();
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
    internal static class ClientExtensions
    {
        internal static void SendMessage(this NBsoft.Sockets.SocketClient cli, Message msg)
        {
            cli.Send(Message.Serialize(msg));
        }
        
    }
}
