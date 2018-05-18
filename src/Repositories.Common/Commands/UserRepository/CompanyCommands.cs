using Contracts;
using Contracts.Models;
using Core.Repositories.Commands.UserRepository;
using Dapper;
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

        public CompanyCommands(Func<IDbConnection> createdDbConnection, Func<string> getLastCreatedId, Func<string> getEncryptionKey)
        {
            _createdDbConnection = createdDbConnection;
            _getLastCreatedId = getLastCreatedId;
            _getEncryptionKey = getEncryptionKey;
        }

        public async Task<IEnumerable<ICompany>> List()
        {
            using (var cnn = _createdDbConnection())
            {
                return (await cnn.QueryAsync<Company>(
                    $"SELECT {Common.GetColumnNames<ICompany>()} FROM [Company]"))
                    .ToList();
            }
        }
        public async Task<ICompany> Get(string companyId)
        {
            if (string.IsNullOrEmpty(companyId))
                throw new ArgumentNullException(nameof(companyId));

            using (var cnn = _createdDbConnection())
            {
                return (await cnn.QueryAsync<Company>(
                    $"SELECT {Common.GetColumnNames<ICompany>()} FROM [Company] WHERE Id = @Id", new { Id = companyId }))
                    .FirstOrDefault();
            }
        }
        public async Task<string> Add(ICompany company)
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
        public async Task<string> Update(ICompany company)
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
        public async Task Delete(string companyId)
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
    }
}
