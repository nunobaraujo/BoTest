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
    [Route("api/job")]    
    public class JobController : Controller, IJobApi
    {
        private readonly IUserManager _userManager;
        public JobController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<string> Add([FromBody] JobRequest request)
        {   
            var repo = await _userManager.ResolveRepository(request.Token);
            if (repo != null)
            {
                var existing = await repo.Job.Get(request.Job.Id);
                if (existing == null)
                    return (await repo.Job.Add(request.Job));
            }
            return null;
        }

        [HttpDelete, Route("{jobId}")]
        public async Task Delete(string jobId, [FromBody]BearerTokenRequest request)
        {
            var repo = await _userManager.ResolveRepository(request.Token);
            if (repo != null)
            {
                var existing = await repo.Job.Get(jobId);
                if (existing != null)
                    await repo.Job.Delete(existing.Id);
            }
        }

        [HttpGet, Route("{jobId}")]
        public async Task<Job> Get(string jobId, BearerTokenRequest request)
        {
            var repo = await _userManager.ResolveRepository(request.Token);
            if (repo != null)
                return (await repo.Job.Get(jobId))
                    .ToDto();
            return null;
        }
        [HttpGet, Route("")]
        public async Task<List<Job>> List(BearerTokenRequest request)
        {
            var repo = await _userManager.ResolveRepository(request.Token);
            if (repo != null)
                return (await repo.Job.List())
                    .Select(x => x.ToDto())
                    .ToList();
            return null;
        }

        [HttpPut, Route("{jobId}")]
        public async Task<string> Update(string jobId, [FromBody]JobRequest request)
        {
            var repo = await _userManager.ResolveRepository(request.Token);
            if (repo != null)
            {
                var existing = await repo.Job.Get(jobId);
                if (existing != null)
                {
                    request.Job.Id = existing.Id;
                    return (await repo.Job.Update(request.Job));
                }
            }
            return null;
        }
                
        [HttpPost, Route("GetByCustomer/{customerId}")]
        public async Task<List<Job>> GetByCustomer(string customerId, [FromBody]BearerTokenRequest request)
        {
            var repo = await _userManager.ResolveRepository(request.Token);
            if (repo != null)
                return (await repo.Job.GetByCustomer(customerId))
                    .Select(x => x.ToDto())
                    .ToList();
            return null;
        }

        [HttpPost, Route("GetByCustomerRoute/{customerRouteId}")]
        public async Task<List<Job>> GetByCustomerRoute(string customerRouteId, [FromBody]BearerTokenRequest request)
        {
            var repo = await _userManager.ResolveRepository(request.Token);
            if (repo != null)
                return (await repo.Job.GetByCustomerRoute(customerRouteId))
                    .Select(x => x.ToDto())
                    .ToList();
            return null;
        }

        [HttpPost, Route("GetByDate/")]
        public async Task<List<Job>> GetByDate([FromBody]DateIntervalRequest  request)
        {
            var repo = await _userManager.ResolveRepository(request.Token);
            if (repo != null)
                return (await repo.Job.GetByDate(request.DateFrom, request.DateTo))
                    .Select(x => x.ToDto())
                    .ToList();
            return null;
        }

        
    }
}
