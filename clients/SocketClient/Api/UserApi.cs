using Contracts;
using Contracts.Api;
using Contracts.Models;
using Contracts.Requests;
using Core.Extensions;
using SocketClient.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<User> Add(CreateUserRequest request)
        {
            _client.Connect();
            return await _client.UserAdd(request);
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

        public async Task<User> Get(IdRequest request)
        {
            _client.Connect();
            return await _client.UserGet(request);
        }

        public async Task<List<CompanyUser>> GetCompanies(BearerTokenRequest request)
        {
            _client.Connect();
            return (await _client.UserGetCompanies(request))
                .ToList();
        }

        public async Task<User> Update(UserRequest request)
        {
            _client.Connect();
            return (await _client.UserUpdate(request)).ToDto();
        }
    }
}
