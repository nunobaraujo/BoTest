using Contracts;
using Contracts.Models;
using Core.Extensions;
using Core.Json;
using Core.Repositories.Commands.UserRepository;
using Dapper;
using NBsoft.Logs.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Common.Commands.UserRepository
{
    internal class UserCommands : IUserCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly Func<string> _getLastCreatedId;
        private readonly Func<string> _getEncryptionKey;
        private readonly ILogger _log;


        public UserCommands(Func<IDbConnection> createdDbConnection, Func<string> getLastCreatedId, Func<string> getEncryptionKey, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _getLastCreatedId = getLastCreatedId;
            _getEncryptionKey = getEncryptionKey;
            _log = log;
        }
        
        public async Task<string> Auth(string userName, string userPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                    throw new ArgumentNullException(nameof(userName));
                if (string.IsNullOrEmpty(userPassword))
                    throw new ArgumentNullException(nameof(userPassword));

                var user = await Get(userName);
                if (user == null)
                    user = await GetByEmail(userName);
                if (user == null)
                    return null;
                return user.CheckPassword(userPassword, _getEncryptionKey()) ? user.UserName : null;
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(UserCommands), nameof(Auth), userName, ex);
                throw;
            }
        }
        public async Task<IEnumerable<IUser>> List()
        {
            try
            {
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<User>(
                        $"SELECT {Common.GetColumnNames<IUser>()} FROM [User]"))
                        .Select(x => x.DecryptUser(_getEncryptionKey()))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(UserCommands), nameof(List), null, ex);
                throw;
            }
        }
        public async Task<IUser> Get(string userName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));
            
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<User>(
                        $"SELECT {Common.GetColumnNames<IUser>()} FROM [User] WHERE UserName = @UserName", new { UserName = userName }))
                        .FirstOrDefault()?
                        .DecryptUser(_getEncryptionKey());
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(UserCommands), nameof(Get), userName, ex);
                throw;
            }
        }
        public async Task<IUser> GetByEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));
            
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<User>(
                        $"SELECT {Common.GetColumnNames<IUser>()} FROM [User] WHERE Email = @Email", new { Email = email.EncryptDPString(_getEncryptionKey()) }))
                        .FirstOrDefault()?
                        .DecryptUser(_getEncryptionKey());
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(UserCommands), nameof(GetByEmail), email, ex);
                throw;
            }
        }
        public async Task<IUser> Add(IUser user)
        {
            try
            {
                if (user == null)
                throw new ArgumentNullException(nameof(user));
            
                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                    // Validate username
                    var userId = await cnn.ExecuteScalarAsync(
                        "SELECT UserName FROM [User] WHERE UserName=@UserName", new { user.UserName }, transaction);
                    if (userId != null)
                        throw new InvalidConstraintException($"Username already exists: {user.UserName}");

                    // Validate email
                    userId = await cnn.ExecuteScalarAsync(
                        "SELECT UserName FROM [User] WHERE Email=@Email", new { user.Email }, transaction);
                    if (userId != null)
                        throw new InvalidConstraintException($"Email already exists: {user.Email}");

                    string query =
                        $"INSERT INTO [User] ({Common.GetColumnNames<IUser>()}) VALUES ({Common.GetFieldNames<IUser>()});";
                    var res = await cnn.ExecuteAsync(query, user.EncryptUser(_getEncryptionKey()), transaction);
                    if (res == 0)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();

                    return user;
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(UserCommands), nameof(Add), user?.ToJson(), ex);
                throw;
            }
        }
        public async Task<IUser> Update(IUser user)
        {
            try
            {
                if (user == null || user.UserName == null)
                throw new ArgumentNullException(nameof(user));
            
                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                    var eUser = user.EncryptUser(_getEncryptionKey());

                    string query =
                        $"UPDATE [User] SET {Common.GetUpdateQueryFields<IUser>("UserName")} WHERE UserName=@UserName";
                    if (await cnn.ExecuteAsync(query, eUser, transaction) != 1)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return user;
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(UserCommands), nameof(Update), user?.ToJson(), ex);
                throw;
            }
        }
        public async Task Delete(string userName)
        {
            try
            {
                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    await cnn.ExecuteAsync(
                        $"DELETE FROM [UserSettings] WHERE UserName = @UserName", new { UserName = userName });
                    await cnn.ExecuteAsync(
                        $"DELETE FROM [CompanyUser] WHERE UserName = @UserName", new { UserName = userName });
                    await cnn.ExecuteAsync(
                        $"DELETE FROM [User] WHERE UserName = @UserName", new { UserName = userName });
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(UserCommands), nameof(Delete), userName, ex);
                throw;
            }
        }

        public async Task SetPassword(string userId, string password)
        {
            try
            {
                var user = await Get(userId);
                if (user != null)
                {
                    var newuser = GeneratePassword(user, password);
                    await Update(newuser);
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(UserCommands), nameof(SetPassword), userId, ex);
                throw;
            }
        }
        public IUser GeneratePassword(IUser user, string password)
        {
            var newUser = user.ToDto();
            newUser.SetPassword(password, _getEncryptionKey());
            return newUser;
        }

        public async Task<IEnumerable<ICompanyUser>> GetCompanies(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new ArgumentNullException(nameof(userId));

                var query = $"SELECT {Common.GetColumnNames<ICompanyUser>()} FROM [CompanyUser] WHERE UserId = @UserId";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<CompanyUser>(query, new { UserId = userId }));
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(UserCommands), nameof(GetCompanies), userId, ex);
                throw;
            }
        }

        public async Task<IEnumerable<string>> UpdateBatch(IEnumerable<IUser> users)
        {
            try
            {
                if (users == null || users.Count() < 1)
                    throw new ArgumentNullException(nameof(users));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    string query =
                        $"UPDATE [User] SET {Common.GetUpdateQueryFields<IUser>("UserName")} WHERE UserName=@UserName";
                    var result = await cnn.ExecuteAsync(query, users.Select(x => x.EncryptUser(_getEncryptionKey())), transaction);
                    if (result != users.Count())
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return users.Select(x => x.UserName);
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(UserCommands), nameof(UpdateBatch), null, ex);
                throw;
            }
        }
    }
}
