using Backend.Infrastucture;
using Contracts.User;
using Core.Repositories;
using Core.Services;
using Core.Services.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class ServerManagerService : IServerManagerService
    {

        private readonly ISessionService _sessionService;
        private readonly IUserRepository _userRepository;

        public ServerManagerService(ISessionService sessionService, IUserRepository userRepository)
        {
            _sessionService = sessionService;
            _userRepository = userRepository;
        }

        public async Task<IUser> GetUserById(string sessionToken, string userId)
        {
            // Only sa user has access to all users.
            // all other users can on request userInfo from their own user.
            var sessionUserId = await _sessionService.GetUserId(sessionToken);
            var sessionUser = await _userRepository.GetUser(sessionUserId);
            if (sessionUser.UserName == Core.Constants.AdminUser || sessionUserId == userId)
                return await _userRepository.GetUser(userId);
            else
                throw new UnauthorizedAccessException($"No access to user {userId}");
        }

        public async Task<string> LogIn(string userName, string password, string userInfo)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                throw new ArgumentException($"{nameof(userName)} and {nameof(password)} cannot be empty.");

            string userId = null;
            // Admin Access
            if (userName == Core.Constants.AdminUser 
                && password == Core.Constants.NbSoftKey)
                userId = (await _userRepository.GetUserByUserName(Core.Constants.AdminUser)).Id;
            else
                userId = await _userRepository.AuthUser(userName, HashPassword(password));                

            if (userId == null)
                throw new UnauthorizedAccessException("Authentication Failed");
            return await _sessionService.CreateNewSession(userId, userInfo);
        }

        public async Task LogOut(string sessionToken)
        {
            await _sessionService.KillSession(sessionToken);
        }

        private static string HashPassword(string plainPassword)
        {
            var res = RijndaelSimple.Encrypt(plainPassword, Core.Constants.NbSoftKey, Constants.Salt, "SHA1", 1, Constants.IV, 128);
            return res;
        }
    }
}
