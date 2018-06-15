using Contracts.Models;
using Contracts.Requests;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Api
{
    public interface ICompanyOptionsApi
    {
        /// <summary>
        /// Get company by Id
        /// </summary>
        [Get("/api/companyoptions/")]
        Task<CompanyOptions> Get(BearerTokenRequest request);

        /// <summary>
        /// Create new user
        /// </summary>
        [Post("/api/companyoptions/")]
        Task<string> AddOrUpdate([Body] CompanyOptionsRequest request);
    }
}
