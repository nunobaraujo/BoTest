using Contracts;
using Contracts.Models;
using Core.Json;
using Core.Repositories.Commands.UserRepository;
using Dapper;
using NBsoft.Logs.Interfaces;
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
        private readonly ILogger _log;

        public UserSettingsCommands(Func<IDbConnection> createdDbConnection, Func<string> getLastCreatedId, Func<string> getEncryptionKey, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _getLastCreatedId = getLastCreatedId;
            _getEncryptionKey = getEncryptionKey;
            _log = log;
        }

        public async Task<string> Add(IUserSettings userSettings)
        {
            try
            {
                if (userSettings == null)
                    throw new ArgumentNullException(nameof(userSettings));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                    // Validate user Id
                    var companyId = await cnn.ExecuteScalarAsync(
                        "SELECT UserName FROM [UserSettings] WHERE UserName=@UserName", new { userSettings.UserName }, transaction);
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
            catch (Exception ex)
            {
                _log?.WriteError(nameof(UserSettingsCommands), nameof(Add), userSettings?.ToJson(), ex);
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
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(UserSettingsCommands), nameof(Delete), userName, ex);
                throw;
            }
        }

        public async Task<IUserSettings> Get(string userName)
        {
            try
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
            catch (Exception ex)
            {
                _log?.WriteError(nameof(UserSettingsCommands), nameof(Get), userName, ex);
                throw;
            }
        }

        public async Task<string> Update(IUserSettings userSettings)
        {
            try
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
            catch (Exception ex)
            {
                _log?.WriteError(nameof(UserSettingsCommands), nameof(Update), userSettings?.ToJson(), ex);
                throw;
            }
        }
    }
}
