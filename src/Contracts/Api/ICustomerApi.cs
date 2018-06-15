using Contracts.Models;
using Contracts.Requests;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Api
{
    public interface ICustomerApi
    {
        /// <summary>
        /// List customers
        /// </summary>
        [Get("/api/customer/List/")]
        Task<List<Customer>> List([Body]BearerTokenRequest request);

        /// <summary>
        /// Create new customer
        /// </summary>
        [Post("/api/customer/")]
        Task<string> Add([Body]CustomerRequest request);

        /// <summary>
        /// Get customer by id
        /// </summary>
        [Get("/api/customer/{customerId}")]
        Task<Customer> Get(string customerId, BearerTokenRequest request);


        /// <summary>
        /// Update customer
        /// </summary>
        [Put("/api/customer/{customerId}")]
        Task<string> Update(string customerId, [Body]CustomerRequest request);

        /// <summary>
        /// Delete customer
        /// </summary>
        [Delete("/api/customer/{customerId}")]
        Task Delete(string customerId, [Body]BearerTokenRequest request);
    }
}
