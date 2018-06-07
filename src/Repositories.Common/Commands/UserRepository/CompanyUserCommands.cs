using Contracts;
using Core.Repositories.Commands.UserRepository;
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

        public Task<string> Add(ICompanyUser companyUser)
        {
            throw new NotImplementedException();
        }

        public Task Delete(string companyUserId)
        {
            throw new NotImplementedException();
        }

        public Task<string> Update(ICompanyUser companyUser)
        {
            throw new NotImplementedException();
        }
    }
}
