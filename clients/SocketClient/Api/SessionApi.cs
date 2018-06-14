using Contracts.Api;
using Contracts.Requests;
using SocketClient.Extensions;
using System.Threading.Tasks;

namespace SocketClient.Api
{
    class SessionApi : ISessionApi
    {
        private readonly Client _client;

        public SessionApi(Client client)
        {
            _client = client;
        }

        public Task<string> GetActiveCompany(BearerTokenRequest request)
        {
            _client.Connect();
            return _client.SessionActiveCompanyGet(request);
        }

        public Task<string> LogIn(LogInRequest request)
        {
            _client.Connect();
            return _client.SessionLogIn(request);
        }

        public Task LogOut(BearerTokenRequest request)
        {
            _client.Connect();
            return _client.SessionLogOut(request);
        }

        public Task SetActiveCompany(IdRequest request)
        {
            _client.Connect();
            return _client.SessionActiveCompanySet(request);
        }
    }
}
