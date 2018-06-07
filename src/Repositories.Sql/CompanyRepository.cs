using Core.Repositories;
using Core.Repositories.Commands.CompanyRepository;
using NBsoft.Logs.Interfaces;
using Repositories.Common;
using System.Data.SqlClient;

namespace Repositories.Sql
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly string _connString;
        private string _encryptionKey;

        public IJobCommands Job { get; }

        public CompanyRepository(string connString, ILogger log)
        {
            _connString = connString;
            _encryptionKey = Core.Constants.NbSoftKey;

            var companyRepositoryFactory = new CompanyRepositoryFactory(() => new SqlConnection(_connString), () => Constants.GetLastInsertedId, () => _encryptionKey, log);
            Job = companyRepositoryFactory.CreateJobCommands();
        }

        public void SetEncryptionKey(string encryptionKey)
        {
            _encryptionKey = encryptionKey;
        }
    }
}
