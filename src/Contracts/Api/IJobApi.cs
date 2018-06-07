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
        /// Get job list
        /// </summary>
        [Get("/api/job/")]
        Task<List<Job>> List(BearerTokenRequest request);

        /// <summary>
        /// Get job by id
        /// </summary>
        [Get("/api/job/{jobId}")]
        Task<Job> Get(string jobId, BearerTokenRequest request);
        /// <summary>
        /// Create new job
        /// </summary>
        [Post("/api/job/")]
        Task<string> Add([Body]JobRequest request);

        /// <summary>
        /// Update job
        /// </summary>
        [Put("/api/job/{jobId}")]
        Task<string> Update(string jobId, [Body]JobRequest request);

        /// <summary>
        /// Delete job
        /// </summary>
        [Delete("/api/job/{jobId}")]
        Task Delete(string jobId, [Body]BearerTokenRequest request);

        /// <summary>
        /// Get jobs by customer
        /// </summary>
        [Post("/api/job/GetByCustomer/{customerId}")]
        Task<List<Job>> GetByCustomer(string customerId, [Body]BearerTokenRequest request);

        /// <summary>
        /// Get jobs by customer route
        /// </summary>
        [Post("/api/job/GetByCustomerRoute/{customerRouteId}")]
        Task<List<Job>> GetByCustomerRoute(string customerRouteId, [Body]BearerTokenRequest request);

        /// <summary>
        /// Get jobs by customer route
        /// </summary>
        [Post("/api/job/GetByDate/")]
        Task<List<Job>> GetByDate([Body]DateIntervalRequest  request);

    }
}
