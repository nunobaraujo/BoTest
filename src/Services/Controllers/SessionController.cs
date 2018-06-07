using Contracts.Api;
using Contracts.Requests;
using Core.Managers;
using Refit;
using Services.Comms;
using System.Threading.Tasks;

namespace Services.Controllers
{
    class SessionController : ISessionApi
    {
        private readonly IUserManager _userManager;
        private readonly IClientConnection _clientConnection;

        public SessionController(IClientConnection clientConnection, IUserManager userManager)
        {
            _userManager = userManager;
            _clientConnection = clientConnection;
        }

        public async Task<string> GetActiveCompany(BearerTokenRequest request)
        {
            return (await _userManager.GetActiveCompany(request.Token)).Id;
        }

        public async Task<string> LogIn([Body] LogInRequest request)
        {
            return await _userManager.LogIn(request.UserName, request.UserPassword, _clientConnection.Tag.ToString());
        }

        public async Task LogOut([Body] BearerTokenRequest request)
        {
            await _userManager.LogOut(request.Token);
        }

        public async Task SetActiveCompany([Body] IdRequest request)
        {
            await _userManager.SetActiveCompany(request.Token, request.Id);
        }
    }
}
