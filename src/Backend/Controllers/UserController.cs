﻿using Contracts;
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
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Authorize]
    [Route("api/user")]
    public class UserController:Controller, IUserApi
    {
        private readonly IUserManager _userManager;

        public UserController(IUserManager userManager)
        {
            _userManager = userManager;
        }
        /// <summary>
        /// Get user by id
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IUser), 200)]
        [ProducesResponseType(204)]
        public async Task<User> Get(IdRequest request)
        {
            return (await _userManager.GetUserById(request.Token, request.Id))?
                .ToDto();
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<User> Add([FromBody]CreateUserRequest request)
        {
            return await _userManager.CreateUser(request.UserName, request.UserPassword, request.Email);
        }

        /// <summary>
        /// Update user info
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<User> Update([FromBody]UserRequest request)
        {
            return await _userManager.UpdateUser(request.Token, request.User);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task Delete(IdRequest request)
        {
            await _userManager.DeleteUser(request.Token, request.Id);
        }

        /// <summary>
        /// Change logged user password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ChangePassword/")]
        public async Task ChangePassword(ChangePasswordRequest request)
        {
            await _userManager.SetPassword(request.Token, request.NewPassword);
        }

        /// <summary>
        /// Get companies this user has access to
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetCompanies/")]
        public async Task<List<CompanyUser>> GetCompanies(BearerTokenRequest request)
        {
            return await _userManager.GetCompanies(request.Token);
        }
    }
}
