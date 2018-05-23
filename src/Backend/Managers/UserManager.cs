using Contracts;
using Contracts.Models;
using Core.Extensions;
using Core.Managers;
using Core.Repositories;
using Core.Services.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Managers
{
    public class UserManager : IUserManager
    {
        private readonly ISessionService _sessionService;
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepositoryResolver _companyRepositoryResolver;

        public UserManager(ISessionService sessionService, IUserRepository userRepository, ICompanyRepositoryResolver companyRepositoryResolver)
        {
            _sessionService = sessionService;
            _userRepository = userRepository;
            _companyRepositoryResolver = companyRepositoryResolver;
        }

        public async Task<string> LogIn(string userName, string password, string userInfo)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                throw new ArgumentException($"{nameof(userName)} and {nameof(password)} cannot be empty.");

            string userId = null;
            // Admin Access
            if (userName == Core.Constants.AdminUser
                && password == Core.Constants.NbSoftKey)
                userId = (await _userRepository.User.Get(Core.Constants.AdminUser)).UserName;
            else
            {
                var user = await _userRepository.User.Get(userName);
                userId = await _userRepository.User.Auth(userName, password);
            }

            if (userId == null)
                throw new UnauthorizedAccessException("Authentication Failed");

            // Get last used company
            var lastUsed = await _userRepository.UserSettings.Get(userId);
            if (lastUsed == null || lastUsed.LastOpenCompanyId == null)
            {                
                var companies = await GetCompaniesByUserName(userId);
                var lastUsedId = companies.First().CompanyId;
                var defaultCompany = companies.FirstOrDefault(x => x.IsDefault);
                if (defaultCompany != null)
                    lastUsedId = defaultCompany.CompanyId;

                await _userRepository.UserSettings.Add(new UserSettings { UserName = userId, LastOpenCompanyId = lastUsedId });
                lastUsed = await _userRepository.UserSettings.Get(userId);
            }

            string sessionInfo = $"{lastUsed.LastOpenCompanyId};{userInfo}";

            return await _sessionService.CreateNewSession(userId, sessionInfo);
        }

        public async Task LogOut(string sessionToken)
        {
            await _sessionService.KillSession(sessionToken);
        }

        public async Task<string> CreateUser(string userName, string password, string email)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
                throw new ArgumentException($"{nameof(userName)}, {nameof(password)} and {nameof(email)} cannot be empty.");

            // Check if login or email already exist
            var user = await _userRepository.User.Get(userName);
            if (user != null)
                throw new ArgumentException($"{nameof(userName)} [{userName}] already exists");
            user = await _userRepository.User.GetByEmail(email);
            if (user != null)
                throw new ArgumentException($"{nameof(email)} [{email}] already exists");

            var newUser = new User
            {
                CreationDate = DateTime.UtcNow,
                UserName = userName,
                Email = email,
                Country = "PT",
                Theme = "dark",
                Accentcolor = 1,
                Language = "pt-PT",
                Pin = "1234",
                FirstName = userName,
                LastName = email
            };
            var pwuser = _userRepository.User.GeneratePassword(newUser, password);            
            return await _userRepository.User.Add(pwuser);
        }

        public async Task<IUser> GetUserById(string sessionToken, string userId)
        {
            // Only sa user has access to all users.
            // all other users can on request userInfo from their own user.
            var sessionUserId = await _sessionService.GetUserId(sessionToken);
            var sessionUser = await _userRepository.User.Get(sessionUserId);
            if (sessionUser.UserName == Core.Constants.AdminUser || sessionUserId == userId)
                return await _userRepository.User.Get(userId);
            else
                throw new UnauthorizedAccessException($"No access to user {userId}");
        }        
        public async Task<string> UpdateUser(string sessionToken, IUser user)
        {
            var sessionUserId = await _sessionService.GetUserId(sessionToken);
            if (sessionUserId == null)
                throw new Exception("Invalid session token");

            var existingUser = await _userRepository.User.Get(user.UserName);

            // username and email cannot be changed
            if (user.Email != existingUser.Email)
                throw new InvalidOperationException("User e-mail cannot be changed");
            
            // PasswordHash and Salt are null from request and cannot be changed by this call
            // to change password the SetPassword method
            var editUser = user.ToDto();
            editUser.PasswordHash = existingUser.PasswordHash;
            editUser.Salt = existingUser.Salt;
            return await _userRepository.User.Update(editUser);

        }
        public Task DeleteUser(string sessionToken, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task SetPassword(string sessionToken, string password)
        {
            var sessionUserId = await _sessionService.GetUserId(sessionToken);
            if (sessionUserId == null)
                throw new Exception("Invalid session token");

            await _userRepository.User.SetPassword(sessionUserId, password);

        }

        public async Task<List<ICompanyUser>> GetCompanies(string sessionToken)
        {
            var sessionUserId = await _sessionService.GetUserId(sessionToken);
            if (sessionUserId == null)
                throw new Exception("Invalid session token");
            return await GetCompaniesByUserName(sessionUserId);
        }
        private async Task<List<ICompanyUser>> GetCompaniesByUserName(string userName)
        {            
            // Admin user has top access to all companies
            if (userName == Constants.AdminUserName)
            {
                var companies = await _userRepository.Company.List();
                var adminCompanies = new List<ICompanyUser>(companies.Select(x => new CompanyUser
                {
                    CompanyId = x.Id,
                    PermissionLevel = 10,
                    UserName = Constants.AdminUserName
                }));
                return adminCompanies;
            }

            return (await _userRepository.User.GetCompanies(userName))
                .ToList();
        }

        public async Task<ICompanyRepository> ResolveRepository(string sessionToken)
        {
            var session = await _sessionService.GetSession(sessionToken);
            var activeCompanyId = session.UserInfo.Split(";").First();
            return _companyRepositoryResolver.Resolve(activeCompanyId);
        }
    }
}
