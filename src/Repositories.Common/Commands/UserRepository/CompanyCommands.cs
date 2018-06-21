using Contracts;
using Contracts.Models;
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
    internal class CompanyCommands: ICompanyCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly Func<string> _getLastCreatedId;
        private readonly Func<string> _getEncryptionKey;
        private readonly ILogger _log;

        public CompanyCommands(Func<IDbConnection> createdDbConnection, Func<string> getLastCreatedId, Func<string> getEncryptionKey, ILogger log )
        {
            _createdDbConnection = createdDbConnection;
            _getLastCreatedId = getLastCreatedId;
            _getEncryptionKey = getEncryptionKey;
            _log = log;
        }

        public async Task<IEnumerable<ICompany>> List()
        {
            try
            {
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Company>(
                        $"SELECT {Common.GetColumnNames<ICompany>()} FROM [Company]"))
                        .Select(x => x.DecryptCompany(_getEncryptionKey()))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(CompanyCommands), nameof(List), null, ex);
                throw;
            }
        }
        public async Task<ICompany> Get(string companyId)
        {
            try
            {
                if (string.IsNullOrEmpty(companyId))
                    throw new ArgumentNullException(nameof(companyId));

                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Company>(
                        $"SELECT {Common.GetColumnNames<ICompany>()} FROM [Company] WHERE Id = @Id", new { Id = companyId }))
                        .FirstOrDefault()?
                        .DecryptCompany(_getEncryptionKey());
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(CompanyCommands), nameof(Get), companyId, ex);
                throw;
            }
        }
        public async Task<string> Add(ICompany company)
        {
            try
            {
                if (company == null)
                    throw new ArgumentNullException(nameof(company));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                    // Validate CompanyCode
                    var companyId = await cnn.ExecuteScalarAsync(
                        "SELECT Id FROM [Company] WHERE Reference=@Reference", new { company.Reference }, transaction);
                    if (companyId != null)
                        throw new InvalidConstraintException($"Reference already exists: {company.Reference }");

                    // Validate TaxIdNumber
                    companyId = await cnn.ExecuteScalarAsync(
                        "SELECT Id FROM [Company] WHERE TaxIdNumber=@TaxIdNumber", new { company.TaxIdNumber }, transaction);
                    if (companyId != null)
                        throw new InvalidConstraintException($"TaxIdNumber already exists: {company.TaxIdNumber}");

                    string query =
                        $"INSERT INTO [Company] ({Common.GetColumnNamesExceptId<ICompany>("Id")}) VALUES ({Common.GetFieldNamesExceptId<ICompany>("Id")}); {_getLastCreatedId()}";
                    var res = await cnn.ExecuteScalarAsync(query, company.EncryptCompany(_getEncryptionKey()), transaction);
                    if (res == null)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();

                    return res.ToString();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(CompanyCommands), nameof(Add), company?.ToJson(), ex);
                throw;
            }
        }
        public async Task<string> Update(ICompany company)
        {
            try
            {
                if (company == null || company.Id == null)
                    throw new ArgumentNullException(nameof(company));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                    var eCompany = company.EncryptCompany(_getEncryptionKey());

                    string query =
                        $"UPDATE [Company] SET {Common.GetUpdateQueryFields<ICompany>("Id")} WHERE Id=@Id";
                    if (await cnn.ExecuteAsync(query, eCompany, transaction) != 1)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return company.Id;
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(CompanyCommands), nameof(Update), company?.ToJson(), ex);
                throw;
            }
        }
        public async Task Delete(string companyId)
        {
            try
            {
                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    await cnn.ExecuteAsync(
                        $"DELETE FROM [CompanyUser] WHERE CompanyId = @CompanyId", new { CompanyId = companyId });
                    await cnn.ExecuteAsync(
                        $"DELETE FROM [Company] WHERE Id = @Id", new { Id = companyId });
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(CompanyCommands), nameof(Delete), companyId, ex);
                throw;
            }
        }

        public async Task<IEnumerable<string>> UpdateBatch(IEnumerable<ICompany> companies)
        {
            try
            {
                if (companies == null || companies.Count() < 1)
                    throw new ArgumentNullException(nameof(companies));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    string query =
                        $"UPDATE [Company] SET {Common.GetUpdateQueryFields<ICompany>("Id")} WHERE Id=@Id";
                    var result = await cnn.ExecuteAsync(query, companies.Select(x => x.EncryptCompany(_getEncryptionKey())), transaction);
                    if (result != companies.Count())
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return companies.Select(x => x.Id);
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(CompanyCommands), nameof(UpdateBatch), null, ex);
                throw;
            }
        }
    }
}
