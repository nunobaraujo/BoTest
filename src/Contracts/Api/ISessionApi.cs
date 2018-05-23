using Contracts.Requests;
using Refit;
using System.Threading.Tasks;

namespace Contracts.Api
{
    public interface ISessionApi
    {
        [Post("/api/session/LogIn/")]
        Task<string> LogIn([Body]LogInRequest request);

        [Delete("/api/session/LogIn/")]
        Task LogOut([Body]BearerTokenRequest request);
    }
}
