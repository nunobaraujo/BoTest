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
                Message nack = new Message(ProtocolCommand.NACK, global::Services.Comms.Sockets.CompressionType.Uncompressed);
                e.Connection.Send(Protocol.EncodeMessageBytes(nack));

                clientConnection.IncomingMessage.Clear();
                await _logger?.WriteErrorAsync(nameof(SocketHost), nameof(ProcessReceivedData), e.Connection.RemoteEndPoint.ToString(), ex01);
            }

            if (isComplete && clientConnection != null)
            {
                clientConnection.IncomingMessage.Clear();
                await ProcessIncomingMessage(clientConnection, tempData);
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
                        object[] pars = received.FormatedBody;
                        long sig = (long)pars[0];
                        Guid tid = (Guid)pars[1];
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
                        var result = new Message
                        {
                            Command = ProtocolCommand.ACK,
                            SubCommand = 0x00
                        };
                        try
                        {
                            SubCommand command = (SubCommand)received.SubCommand;
                            switch (command)
                            {
                                case SubCommand.Unknown:
                                    result.Command = ProtocolCommand.NACK;
                                    break;
                                default:
                                    result.SetInnerbody(await ProcessCommand(connection, (SubCommand)received.SubCommand, received.Reserved, received.FormatedBody));
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            result.SetInnerbody(Protocol.Encode($"ERR:{ex.Message}|STACK:{ex.StackTrace}"));
                            await _logger?.WriteErrorAsync(nameof(SocketHost), nameof(ProcessIncomingMessage), connection.Client.RemoteEndPoint.ToString(), ex);
                        }
                        connection.Send(result);
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
        private async Task<byte[]> ProcessCommand(ClientConnection connection, SubCommand command, byte options, object[] parameters)
        {   
            switch (command)
            {
                #region SessionApi
                case SubCommand.SessionLogIn:
                    return Protocol.Encode(await Session(connection).LogIn((LogInRequest)parameters[0]));
                case SubCommand.SessionLogOut:
                    await Session(connection).LogOut((BearerTokenRequest)parameters[0]);
                    return Protocol.Encode();
                case SubCommand.SessionActiveCompanyGet:
                    return Protocol.Encode(await Session(connection).GetActiveCompany((BearerTokenRequest)parameters[0]));
                case SubCommand.SessionActiveCompanySet:
                    await Session(connection).SetActiveCompany((IdRequest)parameters[0]);
                    return Protocol.Encode();
                #endregion

                #region UserApi
                case SubCommand.UserAdd:
                    return Protocol.Encode(await User(connection).Add((CreateUserRequest)parameters[0]));
                case SubCommand.UserChangePassword:
                    await User(connection).ChangePassword((ChangePasswordRequest)parameters[0]);
                    return Protocol.Encode();
                case SubCommand.UserDelete:
                    await User(connection).Delete((IdRequest)parameters[0]);
                    return Protocol.Encode();
                case SubCommand.UserGet:
                    return Protocol.Encode(await User(connection).Get((IdRequest)parameters[0]));
                case SubCommand.UserGetCompanies:
                    return Protocol.Encode(await User(connection).GetCompanies((BearerTokenRequest)parameters[0]));

                case SubCommand.UserUpdate:
                    return Protocol.Encode(await User(connection).Update((UserRequest)parameters[0]));

                #endregion

                default:
                    return Protocol.Encode(null);
            }
        }

        private ISessionApi Session(IClientConnection connection) => new SessionController(connection, _userManager);
        private IUserApi User(IClientConnection connection) => new UserController(connection, _userManager);

    }     
}
