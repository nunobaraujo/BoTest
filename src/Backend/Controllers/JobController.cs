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
        public Task<string> Add([FromBody] JobRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        public Task Delete([FromBody] GetByIdRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<Job> Get(GetByIdRequest request)
        {
            var repo = await _userManager.ResolveRepository(request.Token);
            if (repo != null)
                return (await repo.Job.Get(request.Id))
                    .ToDto();
            return null;
        }

        [HttpPut]
        public Task<string> Update([FromBody] JobRequest request)
        {
            throw new NotImplementedException();
        }
                
        [HttpPost("/GetByCustomer/")]
        public Task<List<Job>> GetByCustomer(GetByIdRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpPost("/GetByCustomerRoute/")]
        public Task<List<Job>> GetByCustomerRoute(GetByIdRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpPost("/GetByDate/")]
        public Task<List<Job>> GetByDate(GetByDateRequest request)
        {
            throw new NotImplementedException();
        }

        
    }
}
