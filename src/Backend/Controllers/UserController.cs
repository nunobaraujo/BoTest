using Contracts;
using Contracts.Requests;
using Contracts.User;
using Contracts.User.Models;
using Core.Extensions;
using Core.Repositories;
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
    [Route("api/user")]
    public class UserController:Controller, IUserApi
    {
        private readonly IServerManagerService _serverManagerService;

        public UserController(IServerManagerService serverManagerService)
        {
            _serverManagerService = serverManagerService;
        }

        [HttpGet]
        [Route("GetById")]
        [ProducesResponseType(typeof(IUser), 200)]
        public async Task<User> GetById(GetByIdRequest request)
        {
            return (await _serverManagerService.GetUserById(request.Token, request.Id))
                .ToDto();
        }
    }
}
