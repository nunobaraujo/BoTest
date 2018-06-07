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
            throw new NotImplementedException();
        }

        public Task<string> LogIn(LogInRequest request)
        {
            _client.Connect();
            var result = _client.SendCustomMessage(Services.Comms.Sockets.SubCommand.SessionLogIn, 0x00, request);
            return Task.FromResult((string)result.FormatedBody[0]);
        }

        public Task LogOut(BearerTokenRequest request)
        {
            throw new NotImplementedException();
        }

        public Task SetActiveCompany(IdRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
