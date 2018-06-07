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
        /// Get customer by id
        /// </summary>
        [Get("/api/customer/")]
        Task<Customer> Get(IdRequest request);
        /// <summary>
        /// Create new customer
        /// </summary>
        [Post("/api/customer/")]
        Task<string> Add([Body]CustomerRequest request);

        /// <summary>
        /// Update customer
        /// </summary>
        [Put("/api/customer/")]
        Task<string> Update([Body]JobRequest request);

        /// <summary>
        /// Delete customer
        /// </summary>
        [Delete("/api/customer/")]
        Task Delete([Body]IdRequest request);

        /// <summary>
        /// List customers
        /// </summary>
        [Get("/api/customer/List/")]
        Task<List<Customer>> List([Body]BearerTokenRequest request);
    }
}
