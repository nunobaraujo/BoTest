using Core.Repositories.Commands.UserRepository;
using NBsoft.Logs.Interfaces;
using Repositories.Common.Commands.UserRepository;
using System;
using System.Data;

namespace Repositories.Common
{
    public class UserRepositoryFactory
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly Func<string> _getLastCreatedId;
        private readonly Func<string> _getEncryptionKey;
        private readonly ILogger _log;

        public UserRepositoryFactory(Func<IDbConnection> createdDbConnection, Func<string> getLastCreatedId, Func<string> getEncryptionKey, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _getLastCreatedId = getLastCreatedId;
            _getEncryptionKey = getEncryptionKey;
            _log = log;
        }

        public IUserCommands CreateUserCommands()
        {
            return new UserCommands(_createdDbConnection, _getLastCreatedId, _getEncryptionKey, _log);
        }

        public ICompanyCommands CreateCompanyCommands()
        {
            return new CompanyCommands(_createdDbConnection, _getLastCreatedId, _getEncryptionKey, _log);
        }

        public IUserSettingsCommands CreateUserSettingsCommands()
        {
            return new UserSettingsCommands(_createdDbConnection, _getLastCreatedId, _getEncryptionKey, _log);
        }

        public ICompanyUserCommands CreateCompanyUserCommands()
        {
            return new CompanyUserCommands(_createdDbConnection, _getLastCreatedId, _getEncryptionKey, _log);
        }

    }
}
