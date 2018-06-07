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
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Authorize]
    [Route("api/company")]
    public class CompanyController : Controller, ICompanyApi
    {
        private readonly IUserManager _userManager;

        public CompanyController(IUserManager userManager)
        {
            _userManager = userManager;
        }
        [HttpPost]
        public Task<string> Add([FromBody] CompanyRequest request)
        {
            throw new NotImplementedException();
        }
        [HttpDelete]
        public Task<string> Delete(IdRequest request)
        {
            throw new NotImplementedException();
        }
        [HttpGet]
        public async Task<Company> Get(IdRequest request)
        {
            var userRepository = await _userManager.GetUserRepository(request.Token);
            return (await userRepository.Company.Get(request.Id))?.ToDto();
        }
        [HttpPut]
        public Task<string> Update([FromBody] CompanyRequest request)
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
