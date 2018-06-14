using Contracts.Api;
using Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocketClient.Api
{
    class Session : ISessionApi
    {
        private readonly Client _client;

        public Session(Client client)
        {
            _client = client;
        }

        public Task<string> GetActiveCompany(BearerTokenRequest request)
        {
            var result = _client.SendCustomMessage(Services.Comms.Sockets.SubCommand.SessionActiveCompanyGet, 0x00, request);
            return Task.FromResult((string)result.FormatedBody[0]);
        }

        public Task<string> LogIn(LogInRequest request)
        {
            _client.Connect();
            var result = _client.SendCustomMessage(Services.Comms.Sockets.SubCommand.SessionLogIn, 0x00, request);
            return Task.FromResult((string)result.FormatedBody[0]);
        }

        public Task LogOut(BearerTokenRequest request)
        {
            var result = _client.SendCustomMessage(Services.Comms.Sockets.SubCommand.SessionLogOut, 0x00, request);
            return Task.FromResult((string)result.FormatedBody[0]);
        }

        public Task SetActiveCompany(IdRequest request)
        {
            var result = _client.SendCustomMessage(Services.Comms.Sockets.SubCommand.SessionActiveCompanySet, 0x00, request);
            return Task.FromResult(0);
        }
    }
}
