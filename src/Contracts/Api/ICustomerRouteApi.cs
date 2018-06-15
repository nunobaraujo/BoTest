using Contracts.Models;
using Contracts.Requests;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Api
{
    public interface ICustomerRouteApi
    {      
        /// <summary>
        /// Create new customer route
        /// </summary>
        [Post("/api/customerroute/")]
        Task<string> Add([Body]CustomerRouteRequest request);

        /// <summary>
        /// Get customer route by id
        /// </summary>
        [Get("/api/customerroute/{customerRouteId}")]
        Task<CustomerRoute> Get(string customerRouteId, BearerTokenRequest request);


        /// <summary>
        /// Update customer route
        /// </summary>
        [Put("/api/customerroute/{customerRouteId}")]
        Task<string> Update(string customerRouteId, [Body]CustomerRouteRequest request);

        /// <summary>
        /// Delete customer route
        /// </summary>
        [Delete("/api/customerroute/{customerRouteId}")]
        Task Delete(string customerRouteId, [Body]BearerTokenRequest request);

        /// <summary>
        /// List customer routes by customer
        /// </summary>
        [Get("/api/customerroute/GetByCustomer/{customerId}")]
        Task<List<CustomerRoute>> GetByCustomer(string customerId, [Body]BearerTokenRequest request);
    }
}
