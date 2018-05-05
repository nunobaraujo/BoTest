using Contracts.Requests;
using Refit;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ISessionApi
    {
        [Post("/api/Session/LogIn")]
        Task<string> LogIn([Body]LogInRequest request);

        [Delete("/api/Session/LogIn/")]
        Task LogOut([Body]AuthenticatedRequest request);
    }
}
