using Contracts;
using Contracts.Requests;
using Core.Services;
using Core.Services.Session;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]    
    public class SessionController: Controller, ISessionApi
    {
        private readonly IServerManagerService _serverManagerService;
        private readonly ISessionService _sessionService;
        public SessionController(IServerManagerService serverManagerService, ISessionService sessionService)
        {
            _sessionService = sessionService;
            _serverManagerService = serverManagerService;
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
            return await _serverManagerService.LogIn(request.UserName, request.UserPassword, HttpContext.Connection.Id);
        }
        [Authorize]
        [HttpDelete]
        [SwaggerOperation("LogOut")]
        [Route("LogIn/")]        
        public async Task LogOut([FromBody]AuthenticatedRequest request)
        {
            await _serverManagerService.LogOut(request.Token);
        }

        //private static ClaimsIdentity MakeIdentity(string userId, string sessionToken)
        //{
        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, userId),
        //        new Claim(ClaimTypes.Email, userId),
        //        new Claim(ClaimTypes.UserData, sessionToken)
        //    };
        //    return new ClaimsIdentity(claims, "Cookie");
        //}
    }
}
