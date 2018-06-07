using Contracts.Api;
using Contracts.Requests;
using Core.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]    
    public class SessionController: Controller, ISessionApi
    {
        private readonly IUserManager _userManager;
        
        public SessionController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        
        /// <summary>
        /// Checks service is alive
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [SwaggerOperation("LogIn")]
        [Route("LogIn/")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<string> LogIn([FromBody]LogInRequest request)
        {
            return await _userManager.LogIn(request.UserName, request.UserPassword, HttpContext.Connection.Id);
        }
        [Authorize]
        [HttpDelete]
        [SwaggerOperation("LogOut")]
        [Route("LogIn/")]        
        public async Task LogOut([FromBody]BearerTokenRequest request)
        {
            await _userManager.LogOut(request.Token);
        }

        [Authorize]
        [HttpGet]
        [SwaggerOperation("GetActiveCompany")]
        [Route("ActiveCompany/")]
        public async Task<string> GetActiveCompany(BearerTokenRequest request)
        {
            return (await _userManager.GetActiveCompany(request.Token)).Id;
        }
        [Authorize]
        [HttpPost]
        [SwaggerOperation("SetActiveCompany")]
        [Route("ActiveCompany/")]
        public async Task SetActiveCompany([FromBody] IdRequest request)
        {
            await _userManager.SetActiveCompany(request.Token, request.Id);
        }
    }
}
