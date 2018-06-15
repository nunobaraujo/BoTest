using Contracts;
using Contracts.Api;
using Contracts.Models;
using Contracts.Requests;
using Core.Extensions;
using Core.Managers;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Authorize]
    [Route("api/companyoptions")]
    public class CompanyOptionsController : Controller, ICompanyOptionsApi
    {
        //private readonly IUserManager _userManager;

        //public CompanyOptionsController(IUserManager userManager)
        //{
        //    _userManager = userManager;
        //}

        //[HttpPost]
        //public async Task<string> AddOrUpdate([FromBody] CompanyOptionsRequest request)
        //{
        //    var repo = await _userManager.ResolveRepository(request.Token);
        //    if (repo != null)
        //    {
        //        var existing = await repo.Job.Get(request.Job.Id);
        //        if (existing == null)
        //            return (await repo.Job.Add(request.Job));
        //    }
        //    return null;
        //}

        //[HttpGet]
        //public async Task<CompanyOptions> Get(BearerTokenRequest request)
        //{
        //    var repo = await _userManager.ResolveRepository(request.Token);
        //    if (repo != null)
        //        return (await repo.Job.Get(jobId))
        //            .ToDto();
        //    return null;
        //}
        public Task<string> AddOrUpdate([FromBody] CompanyOptionsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<CompanyOptions> Get(BearerTokenRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
