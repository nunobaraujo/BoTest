using Contracts.Models;
using Contracts.Requests;
using Refit;
using System.Threading.Tasks;

namespace Contracts.Api
{
    public interface ICompanyApi
    {
        /// <summary>
        /// Get company by Id
        /// </summary>
        [Get("/api/company/")]
        Task<Company> Get(GetByIdRequest request);
    
        /// <summary>
        /// Create new user
        /// </summary>
        [Post("/api/company/")]
        Task<string> Add([Body] CompanyRequest request);

        /// <summary>
        /// Update user info
        /// </summary>
        [Put("/api/company/")]
        Task<string> Update([Body] CompanyRequest request);

        /// <summary>
        /// Delete user
        /// </summary>
        [Delete("/api/company/")]
        Task<string> Delete([Body] GetByIdRequest request);

        /// <summary>
        /// Asing user to company
        /// </summary>
        [Post("/api/company/AssignUser")]
        Task<string> AssignUser([Body] AssignCompanyUserRequest request);

    }
}
