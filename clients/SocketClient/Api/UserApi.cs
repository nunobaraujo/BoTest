using Contracts;
using Contracts.Api;
using Contracts.Requests;
using SocketClient.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocketClient.Api
{
    class UserApi : IUserApi
    {
        private readonly Client _client;

        public UserApi(Client client)
        {
            _client = client;
        }

        public Task<IUser> Add(CreateUserRequest request)
        {
            _client.Connect();
            return _client.UserAdd(request);
        }

        public Task ChangePassword(ChangePasswordRequest request)
        {
            _client.Connect();
            return _client.UserChangePassword(request);
        }

        public Task Delete(IdRequest request)
        {
            _client.Connect();
            return _client.UserDelete(request);
        }

        public Task<IUser> Get(IdRequest request)
        {
            _client.Connect();
            return _client.UserGet(request);
        }

        public Task<List<ICompanyUser>> GetCompanies(BearerTokenRequest request)
        {
            _client.Connect();
            return _client.UserGetCompanies(request);
        }

        public Task<IUser> Update(UserRequest request)
        {
            _client.Connect();
            return _client.UserUpdate(request);
        }
    }
}
