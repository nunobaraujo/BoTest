using Contracts;
using Contracts.Models;
using Core.Repositories.Commands.UserRepository;
using Dapper;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Common.Commands.UserRepository
{
    internal class UserSettingsCommands : IUserSettingsCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly Func<string> _getLastCreatedId;
        private readonly Func<string> _getEncryptionKey;

        public UserSettingsCommands(Func<IDbConnection> createdDbConnection, Func<string> getLastCreatedId, Func<string> getEncryptionKey)
        {
            _createdDbConnection = createdDbConnection;
            _getLastCreatedId = getLastCreatedId;
            _getEncryptionKey = getEncryptionKey;
        }

        public async Task<string> Add(IUserSettings userSettings)
        {
            if (userSettings == null)
                throw new ArgumentNullException(nameof(userSettings));

            using (var cnn = _createdDbConnection())
            {
                cnn.Open();
                var transaction = cnn.BeginTransaction();
                // Validate user Id
                var companyId = await cnn.ExecuteScalarAsync(
                    "SELECT UserName FROM [UserSettings] WHERE UserName=@UserName", new {userSettings.UserName}, transaction);
                if (companyId != null)
                    throw new InvalidConstraintException($"UserSettings already exists for user: {userSettings.UserName}");

                string query =
                    $"INSERT INTO [UserSettings] ({Common.GetColumnNames<IUserSettings>()}) VALUES ({Common.GetFieldNames<IUserSettings>()});";
                var res = await cnn.ExecuteAsync(query, userSettings, transaction);
                if (res != 1)
                    throw new Exception($"ExecuteAsync failed: {query}");
                transaction.Commit();

                return userSettings.UserName;
            }
        }

        public async Task Delete(string userName)
        {
            using (var cnn = _createdDbConnection())
            {
                cnn.Open();                
                await cnn.ExecuteAsync(
                    $"DELETE FROM [UserSettings] WHERE UserName = @UserName", new { UserName = userName });
            }
        }

        public async Task<IUserSettings> Get(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(nameof(userName));

            using (var cnn = _createdDbConnection())
            {
                return (await cnn.QueryAsync<UserSettings>(
                    $"SELECT {Common.GetColumnNames<IUserSettings>()} FROM [UserSettings] WHERE UserName = @UserName", new { UserName = userName }))
                    .FirstOrDefault();
            }
        }

        public async Task<string> Update(IUserSettings userSettings)
        {
            if (userSettings == null || userSettings.UserName == null || userSettings.LastOpenCompanyId == null)
                throw new ArgumentNullException(nameof(userSettings));

            using (var cnn = _createdDbConnection())
            {
                cnn.Open();
                var transaction = cnn.BeginTransaction();
                
                string query =
                    $"UPDATE [UserSettings] SET {Common.GetUpdateQueryFields<IUserSettings>("UserName")} WHERE UserName=@UserName";
                if (await cnn.ExecuteAsync(query, userSettings, transaction) != 1)
                    throw new Exception($"ExecuteAsync failed: {query}");
                transaction.Commit();
                return userSettings.UserName;
            }
        }
    }
}
