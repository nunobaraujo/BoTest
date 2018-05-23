
using Contracts.Requests;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Api
{
    public interface IUserApi
    {
        /// <summary>
        /// Get user by id
        /// </summary>
        [Get("/api/user/")]
        Task<Models.User> Get(GetByIdRequest request);
        /// <summary>
        /// Create new user
        /// </summary>
        [Post("/api/user/")]
        Task<string> Add([Body]CreateUserRequest request);

        /// <summary>
        /// Update user info
        /// </summary>
        [Put("/api/user/")]
        Task<string> Update([Body]UpdateUserRequest request);

        /// <summary>
        /// Delete user
        /// </summary>
        [Delete("/api/user/")]
        Task Delete(GetByIdRequest request);

        /// <summary>
        /// Change User Password
        /// </summary>
        [Post("/api/user/ChangePassword/")]
        Task ChangePassword(ChangePasswordRequest request);

        /// <summary>
        /// Get companies this user has access to
        /// </summary>
        [Post("/api/user/GetCompanies/")]
        Task<List<ICompanyUser>> GetCompanies(BearerTokenRequest request);
    }
}
