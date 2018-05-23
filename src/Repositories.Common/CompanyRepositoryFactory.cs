using Core.Repositories.Commands.CompanyRepository;
using Repositories.Common.Commands.CompanyRepository;
using Repositories.Common.Commands.UserRepository;
using System;
using System.Data;

namespace Repositories.Common
{
    public class CompanyRepositoryFactory
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly Func<string> _getLastCreatedId;
        private readonly Func<string> _getEncryptionKey;

        public CompanyRepositoryFactory(Func<IDbConnection> createdDbConnection, Func<string> getLastCreatedId, Func<string> getEncryptionKey)
        {
            _createdDbConnection = createdDbConnection;
            _getLastCreatedId = getLastCreatedId;
            _getEncryptionKey = getEncryptionKey;
        }

        public IJobCommands CreateJobCommands()
        {
            return new JobCommands(_createdDbConnection, _getLastCreatedId, _getEncryptionKey);
        }
    }
}
