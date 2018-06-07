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

        [Get("/api/session/ActiveCompany/")]
        Task<string> GetActiveCompany(BearerTokenRequest request);

        [Post("/api/session/ActiveCompany/")]
        Task SetActiveCompany([Body]IdRequest request);
    }
}
