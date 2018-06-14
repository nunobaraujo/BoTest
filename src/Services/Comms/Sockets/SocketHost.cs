using Contracts.Api;
using Contracts.Requests;
using Core.Managers;
using NBsoft.Logs.Interfaces;
using NBsoft.Sockets;
using Services.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Services.Comms.Sockets
{
    public class SocketHost : SocketServer, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IUserManager _userManager;
        private readonly List<ClientConnection> _connectedClients;
        
        public SocketHost(IPEndPoint endPoint, ILogger logger, IUserManager userManager)
            : base(endPoint)
        {
            _logger = logger;
            _userManager = userManager;

            _connectedClients = new List<ClientConnection>();
            


            Listen();
            _logger?.WriteInfoAsync(nameof(SocketHost), "Constructor", endPoint.ToString(), $"Socket Server started on port {endPoint.Port}");
        }
        ~SocketHost()
        {
            ClearResources();
        }

        protected override void OnClientConnected(ClientEventArgs e)
        {
            var newclient = new ClientConnection
            {
                Client = e.Connection,
                ConnectionStart = DateTime.UtcNow,
                IsValid = false,
                IncomingMessage = new List<byte>()
            };
            _connectedClients.Add(newclient);
            _logger?.WriteInfoAsync(nameof(SocketHost), nameof(OnClientConnected), e.Connection.RemoteEndPoint.ToString(), $"Client connected: {newclient.Client.RemoteEndPoint}");
        }
        protected override void OnClientDisconnected(ClientEventArgs e)
        {
            var cli = _connectedClients.FirstOrDefault(x => x.Client == e.Connection);
            if (cli != null)
            {
                _logger?.WriteInfoAsync(nameof(SocketHost), nameof(OnClientDisconnected), e.Connection?.RemoteEndPoint?.ToString(), $"Client disconnected: {cli.Client.RemoteEndPoint}");
                _connectedClients.Remove(cli);
            }
        }
        protected override void OnDataReceived(ClientEventArgs e)
        {
            Task.Run(() => ProcessReceivedData(e));
        }

        public void Dispose()
        {
            ClearResources();
        }

        private void ClearResources()
        {
            AbortListen();
        }

        private async Task ProcessReceivedData(ClientEventArgs e)
        {
            bool isComplete = false;
            ClientConnection clientConnection = null;
            byte[] tempData = null;
            try
            {
                int ct = 0;
                do
                {
                    ct++;
                    clientConnection = _connectedClients.FirstOrDefault(x => x.Client == e.Connection);
                    if (clientConnection != null)
                        break;
                    else
                    {
                        await _logger?.WriteWarningAsync(nameof(SocketHost), nameof(ProcessReceivedData), e.Connection.RemoteEndPoint.ToString(), $"Client Doesn't Exist: {e.Connection.RemoteEndPoint} Waiting...");
                        System.Threading.Thread.Sleep(100);
                    }
                } while (ct < 5);
                if (clientConnection == null)
                    tempData = e.Data;
                else
                {
                    clientConnection.IncomingMessage.AddRange(e.Data);
                    tempData = clientConnection.IncomingMessage.ToArray();
                }

                isComplete = Protocol.IsMessageComplete(tempData);
            }
            catch (Exception ex01)
            {
                isComplete = false;                
                e.Connection.Send(Protocol.EncodeMessageBytes(new Message(ProtocolCommand.NACK, CompressionType.Uncompressed)));

                clientConnection.IncomingMessage.Clear();
                await _logger?.WriteErrorAsync(nameof(SocketHost), nameof(ProcessReceivedData), e.Connection.RemoteEndPoint.ToString(), ex01);
            }

            try
            {
                if (isComplete && clientConnection != null)
                {
                    clientConnection.IncomingMessage.Clear();
                    await ProcessIncomingMessage(clientConnection, tempData);
                }
            }
            catch (Exception ex)
            {
                await _logger?.WriteErrorAsync(nameof(SocketHost), nameof(ProcessReceivedData), e.Connection.RemoteEndPoint.ToString(), ex);
                SendError(clientConnection, ex);
            }

        }

        private async Task ProcessIncomingMessage(ClientConnection connection, byte[] message)
        {
            Message received;
            try { received = Protocol.DecodeMessageBytes(message); }
            catch (Exception ex01)
            {
                connection.SendNack();
                await _logger?.WriteErrorAsync(nameof(SocketHost), nameof(ProcessIncomingMessage), connection.Client.RemoteEndPoint.ToString(), ex01);
                return;
            }

            switch (received.Command)
            {
                case ProtocolCommand.Unknown:
                    #region Unknown
                    // Comando desconhecido. Responder NACK
                    connection.SendNack();
                    break;
                #endregion
                // Resposta a um comando
                case ProtocolCommand.NACK:
                case ProtocolCommand.ACK:
                    break;
                case ProtocolCommand.Validate:
                    #region Validate Client
                    bool isvalid = true;
                    if (received.SubCommand != 0xFF)
                        isvalid = false;
                    else
                    {
                        long sig = received.GetParameter<long>();
                        Guid tid = received.GetParameter<Guid>(1); 
                        if (sig != Protocol.ClientSignature)
                            isvalid = false;

                        connection.Tag = tid;
                    }

                    connection.IsValid = isvalid;
                    if (connection.IsValid)
                        connection.SendAck();
                    else
                    {
                        await _logger?.WriteErrorAsync(nameof(SocketHost), nameof(ProcessIncomingMessage), connection.Client.RemoteEndPoint.ToString(),
                            new ProtocolViolationException("Client rejected: Invalid Signature."));
                        connection.SendNack();
                    }

                    break;
                #endregion
                case ProtocolCommand.HeartBeat:
                    #region HeartBeat
                    if (connection.IsValid)
                        connection.SendAck();
                    else
                    {
                        await _logger?.WriteErrorAsync(nameof(SocketHost), nameof(ProcessIncomingMessage), connection.Client.RemoteEndPoint.ToString(),
                            new ProtocolViolationException("Client rejected: Invalid Signature."));
                        connection.SendNack();
                    }
                    break;
                #endregion
                case ProtocolCommand.ClientList:
                    #region ClientList
                    if (connection.IsValid)
                    {
                        Message reval = new Message();
                        reval.Command = ProtocolCommand.ACK;
                        reval.SubCommand = 0x00;
                        Array a = _connectedClients.ToArray();
                        reval.SetInnerbody(Protocol.Encode(a));
                        connection.Send(reval);
                    }
                    else
                    {
                        await _logger?.WriteErrorAsync(nameof(SocketHost), nameof(ProcessIncomingMessage), connection.Client.RemoteEndPoint.ToString(),
                            new ProtocolViolationException("Client rejected: Invalid Signature."));
                        connection.SendNack();
                    }
                    break;
                #endregion
                case ProtocolCommand.Custom:
                    #region Custom
                    // Not Protocol, process subcommand
                    if (connection.IsValid)
                    {   
                        try
                        {
                            var result = new Message
                            {
                                Command = ProtocolCommand.ACK,
                                SubCommand = 0x00
                            };
                            SubCommand command = (SubCommand)received.SubCommand;
                            switch (command)
                            {
                                case SubCommand.Unknown:
                                    result.Command = ProtocolCommand.NACK;
                                    break;
                                default:
                                    //result.SetInnerbody(await ProcessCommand(connection, (SubCommand)received.SubCommand, received.Reserved, received.InnerBody));
                                    result.SetInnerbody(await ProcessCommand(connection, received));
                                    break;
                            }
                            connection.Send(result);
                        }
                        catch (Exception ex)
                        {
                            await _logger?.WriteErrorAsync(nameof(SocketHost), nameof(ProcessIncomingMessage), connection.Client.RemoteEndPoint.ToString(), ex);
                            SendError(connection, ex);
                        }
                    }
                    else
                    {
                        await _logger?.WriteErrorAsync(nameof(SocketHost), nameof(ProcessIncomingMessage), connection.Client.RemoteEndPoint.ToString(),
                            new ProtocolViolationException("Client rejected: Invalid Signature."));
                        connection.SendNack();
                    }
                    break;
                #endregion
                default:
                    break;
            }

        }
        private async Task<byte[]> ProcessCommand(ClientConnection connection, Message msg)
        {   
            switch ((SubCommand)msg.SubCommand)
            {
                #region SessionApi
                case SubCommand.SessionLogIn:
                    return Protocol.Encode(await Session(connection).LogIn(msg.GetParameter<LogInRequest>()));
                case SubCommand.SessionLogOut:
                    await Session(connection).LogOut(msg.GetParameter<BearerTokenRequest>());
                    return new byte[0];
                case SubCommand.SessionActiveCompanyGet:
                    return Protocol.Encode(await Session(connection).GetActiveCompany(msg.GetParameter<BearerTokenRequest>()));
                case SubCommand.SessionActiveCompanySet:
                    await Session(connection).SetActiveCompany(msg.GetParameter<IdRequest>());
                    return new byte[0];
                #endregion

                #region UserApi
                case SubCommand.UserAdd:
                    return Protocol.Encode(await User(connection).Add(msg.GetParameter<CreateUserRequest>()));
                case SubCommand.UserChangePassword:
                    await User(connection).ChangePassword(msg.GetParameter<ChangePasswordRequest>());
                    return new byte[0];
                case SubCommand.UserDelete:
                    await User(connection).Delete(msg.GetParameter<IdRequest>());
                    return new byte[0];
                case SubCommand.UserGet:
                    return Protocol.Encode(await User(connection).Get(msg.GetParameter<IdRequest>()));
                case SubCommand.UserGetCompanies:
                    return Protocol.Encode(await User(connection).GetCompanies(msg.GetParameter<BearerTokenRequest>()));

                case SubCommand.UserUpdate:
                    return Protocol.Encode(await User(connection).Update(msg.GetParameter<UserRequest>()));

                #endregion

                default:
                    return new byte[0];
            }
        }
        
        private void SendError(ClientConnection clientConnection, Exception ex)
        {
            var msg = new Message(ProtocolCommand.ACK, CompressionType.Uncompressed);
            msg.SetInnerbody(Protocol.Encode(new List<byte[]>() { ModelSerializer.Serialize($"ERR:{ex.Message}|STACK:{ex.StackTrace}") }));
            clientConnection.Send(msg);
        }

        private ISessionApi Session(IClientConnection connection) => new SessionController(connection, _userManager);
        private IUserApi User(IClientConnection connection) => new UserController(connection, _userManager);

    }     
}
