using Contracts;
using Contracts.Api;
using Contracts.Models;
using Contracts.Requests;
using Core.Extensions;
using Core.Managers;
using Refit;
using Services.Comms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Controllers
{
    class UserController:IUserApi
    {
        private readonly IUserManager _userManager;
        private readonly IClientConnection _clientConnection;

        public UserController(IClientConnection clientConnection, IUserManager userManager)
        {
            _userManager = userManager;
            _clientConnection = clientConnection;
        }

        public async Task<User> Add([Body] CreateUserRequest request)
        {
            return await _userManager.CreateUser(request.UserName, request.UserPassword, request.Email);
        }

        public async Task ChangePassword(ChangePasswordRequest request)
        {
            await _userManager.SetPassword(request.Token, request.NewPassword);
        }

        public async Task Delete(IdRequest request)
        {
            await _userManager.DeleteUser(request.Token, request.Id);
        }

        public async Task<User> Get(IdRequest request)
        {
            return (await _userManager.GetUserById(request.Token, request.Id))?
                .ToDto();
        }

        public async Task<List<CompanyUser>> GetCompanies(BearerTokenRequest request)
        {
            return await _userManager.GetCompanies(request.Token);
        }

        public async Task<User> Update([Body] UserRequest request)
        {
            return await _userManager.UpdateUser(request.Token, request.User);
        }
    }
}
