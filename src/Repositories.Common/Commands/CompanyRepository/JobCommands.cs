using Contracts;
using Contracts.Models;
using Core.Repositories.Commands.CompanyRepository;
using Dapper;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Common.Commands.CompanyRepository
{
    internal class JobCommands: IJobCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly Func<string> _getLastCreatedId;
        private readonly Func<string> _getEncryptionKey;

        public JobCommands(Func<IDbConnection> createdDbConnection, Func<string> getLastCreatedId, Func<string> getEncryptionKey)
        {
            _createdDbConnection = createdDbConnection;
            _getLastCreatedId = getLastCreatedId;
            _getEncryptionKey = getEncryptionKey;
        }

        public async Task<IJob> Get(string jobId)
        {
            if (string.IsNullOrEmpty(jobId))
                throw new ArgumentNullException(nameof(jobId));

            var query = $"SELECT {Common.GetColumnNames<IJob>()} FROM [Job] WHERE Id = @Id";
            using (var cnn = _createdDbConnection())
            {
                return (await cnn.QueryAsync<Job>(query, new { Id = jobId }))
                    .FirstOrDefault();
            }
        }
        public Task<string> Add(IJob job)
        {
            throw new NotImplementedException();
        }
        public Task Delete(string jobId)
        {
            throw new NotImplementedException();
        }
        public Task<string> Update(IJob job)
        {
            throw new NotImplementedException();
        }

    }
}
