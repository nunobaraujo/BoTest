using Contracts;
using Core.Json;
using Core.Repositories.Commands.UserRepository;
using Dapper;
using NBsoft.Logs.Interfaces;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Repositories.Common.Commands.UserRepository
{
    internal class CompanyUserCommands : ICompanyUserCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly Func<string> _getLastCreatedId;
        private readonly Func<string> _getEncryptionKey;
        private readonly ILogger _log;

        public CompanyUserCommands(Func<IDbConnection> createdDbConnection, Func<string> getLastCreatedId, Func<string> getEncryptionKey, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _getLastCreatedId = getLastCreatedId;
            _getEncryptionKey = getEncryptionKey;
            _log = log;
        }

        public async Task<string> Add(ICompanyUser companyUser)
        {
            try
            {
                if (companyUser == null)
                    throw new ArgumentNullException(nameof(companyUser));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                    // Validate CompanyCode
                    var companyUserId = await cnn.ExecuteScalarAsync(
                        "SELECT Id FROM [CompanyUser] WHERE Id=@Id", new { companyUser.Id }, transaction);
                    if (companyUserId != null)
                        throw new InvalidConstraintException($"Id already exists: {companyUserId}");
                    
                    string query =
                        $"INSERT INTO [CompanyUser] ({Common.GetColumnNames<ICompanyUser>()}) VALUES ({Common.GetFieldNames<ICompanyUser>()});";
                    var res = await cnn.ExecuteAsync(query, companyUser, transaction);
                    if (res != 1)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();

                    return res.ToString();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(CompanyUserCommands), nameof(Add), companyUser?.ToJson(), ex);
                throw;
            }
        }

        public async Task Delete(string companyUserId)
        {
            try
            {
                if (companyUserId == null)
                    throw new ArgumentNullException(nameof(companyUserId));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                                        
                  await cnn.ExecuteAsync(
                        $"DELETE FROM [CompanyUser] WHERE Id = @Id", new { Id = companyUserId }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(CompanyUserCommands), nameof(Delete), companyUserId, ex);
                throw;
            }
        }

        public async Task<string> Update(ICompanyUser companyUser)
        {
            try
            {
                if (companyUser == null)
                    throw new ArgumentNullException(nameof(companyUser));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    string query =
                        $"UPDATE [CompanyUser] SET {Common.GetUpdateQueryFields<ICompanyUser>("Id")} WHERE Id=@Id";

                    if (await cnn.ExecuteAsync(query, companyUser, transaction) != 1)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return companyUser.ToString();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(CompanyUserCommands), nameof(Update), companyUser?.ToJson(), ex);
                throw;
            }
        }
    }
}
