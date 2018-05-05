
using Contracts.Requests;
using Refit;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUserApi
    {
        /// <summary>
        /// Get user by Id
        /// </summary>
        [Get("/api/user/GetById/")]
        Task<User.Models.User> GetById(GetByIdRequest request);
    }
}
