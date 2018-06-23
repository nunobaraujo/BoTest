using Contracts.Api;
using Contracts.Models;
using Contracts.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Managers;
using Core.Extensions;
using System.Linq;

namespace Backend.Controllers
{
    [Authorize]
    [Route("api/customer")]    
    public class CustomerController : Controller, ICustomerApi
    {
        private readonly IUserManager _userManager;
        public CustomerController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<string> Add([FromBody] CustomerRequest request)
        {   
            var repo = await _userManager.ResolveRepository(request.Token);
            if (repo != null)
            {
                return await repo.Customer.Add(request.Customer);
            }
            return null;
        }

        [HttpDelete, Route("{customerId}")]
        public async Task Delete(string customerId, [FromBody]BearerTokenRequest request)
        {
            var repo = await _userManager.ResolveRepository(request.Token);
            if (repo != null)
            {
                var existing = await repo.Job.Get(customerId);
                if (existing != null)
                    await repo.Job.Delete(existing.Id);
            }
        }

        [HttpGet, Route("{customerId}")]
        public async Task<Customer> Get(string customerId, BearerTokenRequest request)
        {
            var repo = await _userManager.ResolveRepository(request.Token);
            if (repo != null)
                return (await repo.Customer.Get(customerId))
                    .ToDto();
            return null;
        }

        public async Task<List<Customer>> List([FromBody] BearerTokenRequest request)
        {
            var repo = await _userManager.ResolveRepository(request.Token);
            if (repo != null)
                return (await repo.Customer.List())
                    .Select(x => x.ToDto())
                    .ToDto();
            return null;
        }

        [HttpPut, Route("{customerId}")]
        public async Task<string> Update(string customerId, [FromBody]CustomerRequest request)
        {
            var repo = await _userManager.ResolveRepository(request.Token);
            if (repo != null)
            {
                var existing = await repo.Customer.Get(customerId);
                if (existing != null)
                {
                    request.Customer.Id = existing.Id;
                    return (await repo.Customer.Update(request.Customer));
                }
            }
            return null;
        }

        
    }
}
