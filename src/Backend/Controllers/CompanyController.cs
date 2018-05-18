using Contracts;
using Contracts.Api;
using Contracts.Models;
using Contracts.Requests;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Authorize]
    [Route("api/company")]
    public class CompanyController : Controller, ICompanyApi
    {
        //private readonly IServerManagerService _serverManagerService;

        public CompanyController()
        {
        }
        [HttpPost]
        public Task<string> AddCompany([FromBody] CompanyRequest request)
        {
            throw new NotImplementedException();
        }
        [HttpDelete]
        public Task<string> DeleteCompany(GetByIdRequest request)
        {
            throw new NotImplementedException();
        }
        [HttpGet]
        public Task<Company> GetCompanyById(GetByIdRequest request)
        {
            throw new NotImplementedException();
        }
        [HttpPut]
        public Task<string> UpdateCompany([FromBody] CompanyRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asing user to company
        /// </summary>
        [HttpPost("/api/company/AssignUser")]
        public Task<string> AssignUser([FromBody] AssignCompanyUserRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
