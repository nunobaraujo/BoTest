using Contracts;
using Contracts.Models;
using Core.Json;
using Core.Repositories.Commands.CompanyRepository;
using Dapper;
using NBsoft.Logs.Interfaces;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Common.Commands.CompanyRepository
{
    internal class CompanyOptionsCommands : ICompanyOptionsCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public CompanyOptionsCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<ICompanyOptions> Get()
        {
            try
            {
                var query = $"SELECT {Common.GetColumnNames<ICompanyOptions>()} FROM [CompanyOptions] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<CompanyOptions>(query, new { Id = "1" }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobCommands), nameof(Get), null, ex);
                throw;
            }
        }

        public async Task Set(ICompanyOptions companyOptions)
        {
            try
            {
                if (companyOptions == null && companyOptions.Id != "1")
                    throw new ArgumentNullException(nameof(companyOptions));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                    
                    var id = await cnn.ExecuteScalarAsync(
                        "SELECT Id FROM [CompanyOptions] WHERE Id='1'",null, transaction);


                    string query = (id != null) 
                        ? $"UPDATE [CompanyOptions] SET {Common.GetUpdateQueryFields<ICompanyOptions>("Id")} WHERE Id='1'"
                        : $"INSERT INTO [CompanyOptions] ({Common.GetColumnNames<ICompanyOptions>()}) VALUES ({Common.GetFieldNames<ICompanyOptions>()});";
                    
                    var res = await cnn.ExecuteAsync(query, companyOptions, transaction);
                    if (res != 1)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobCommands), nameof(Set), companyOptions?.ToJson(), ex);
                throw;
            }
        }
    }
}
