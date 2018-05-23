using Contracts.Models;
using Contracts.Requests;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Api
{
    public interface IJobApi
    {
        /// <summary>
        /// Get job by id
        /// </summary>
        [Get("/api/job/")]
        Task<Job> Get(GetByIdRequest request);
        /// <summary>
        /// Create new job
        /// </summary>
        [Post("/api/job/")]
        Task<string> Add([Body]JobRequest request);

        /// <summary>
        /// Update job
        /// </summary>
        [Put("/api/job/")]
        Task<string> Update([Body]JobRequest request);

        /// <summary>
        /// Delete job
        /// </summary>
        [Delete("/api/job/")]
        Task Delete([Body]GetByIdRequest request);

        /// <summary>
        /// Get jobs by customer
        /// </summary>
        [Post("/api/job/GetByCustomer/")]
        Task<List<Job>> GetByCustomer(GetByIdRequest request);

        /// <summary>
        /// Get jobs by customer route
        /// </summary>
        [Post("/api/job/GetByCustomerRoute/")]
        Task<List<Job>> GetByCustomerRoute(GetByIdRequest request);

        /// <summary>
        /// Get jobs by customer route
        /// </summary>
        [Post("/api/job/GetByDate/")]
        Task<List<Job>> GetByDate(GetByDateRequest request);

    }
}
