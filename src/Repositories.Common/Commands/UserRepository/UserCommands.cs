using Contracts;
using Contracts.Models;
using Core.Extensions;
using Core.Repositories.Commands.UserRepository;
using Dapper;
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
        
        public UserCommands(Func<IDbConnection> createdDbConnection, Func<string> getLastCreatedId, Func<string> getEncryptionKey)
        {
            _createdDbConnection = createdDbConnection;
            _getLastCreatedId = getLastCreatedId;
            _getEncryptionKey = getEncryptionKey;
        }
        public async Task<string> Auth(string userName, string userPassword)
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
        public async Task<IUser> Get(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            using (var cnn = _createdDbConnection())
            {
                return (await cnn.QueryAsync<User>(
                    $"SELECT {Common.GetColumnNames<IUser>()} FROM [User] WHERE UserName = @UserName", new { UserName = userName }))
                    .FirstOrDefault();
            }
        }
        public async Task<IUser> GetByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException(nameof(email));

            using (var cnn = _createdDbConnection())
            {
                return (await cnn.QueryAsync<User>(
                    $"SELECT {Common.GetColumnNames<IUser>()} FROM [User] WHERE Email = @Email", new { Email = email.EncryptDPString(_getEncryptionKey()) }))
                    .FirstOrDefault();
            }
        }
        public async Task<string> Add(IUser user)
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

                return user.UserName;
            }
        }
        public async Task<string> Update(IUser user)
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
                return user.UserName;
            }
        }
        public async Task Delete(string userName)
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

        public async Task SetPassword(string userId, string password)
        {
            var user = await Get(userId);
            if (user != null)
            {
                var newuser = GeneratePassword(user, password);
                await Update(newuser);
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
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var query = $"SELECT {Common.GetColumnNames<ICompanyUser>()} FROM [CompanyUser] WHERE UserId = @UserId";
            using (var cnn = _createdDbConnection())
            {
                return (await cnn.QueryAsync<CompanyUser>(query, new { UserId = userId }));
            }

        }
    }
}
