using Core.Repositories.Commands.UserRepository;
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

        public UserRepositoryFactory(Func<IDbConnection> createdDbConnection, Func<string> getLastCreatedId, Func<string> getEncryptionKey)
        {
            _createdDbConnection = createdDbConnection;
            _getLastCreatedId = getLastCreatedId;
            _getEncryptionKey = getEncryptionKey;
        }

        public IUserCommands CreateUserCommands()
        {
            return new UserCommands(_createdDbConnection, _getLastCreatedId, _getEncryptionKey);
        }

        public ICompanyCommands CreateCompanyCommands()
        {
            return new CompanyCommands(_createdDbConnection, _getLastCreatedId, _getEncryptionKey);
        }

        public IUserSettingsCommands CreateUserSettingsCommands()
        {
            return new UserSettingsCommands(_createdDbConnection, _getLastCreatedId, _getEncryptionKey);
        }

        public ICompanyUserCommands CreateCompanyUserCommands()
        {
            return new CompanyUserCommands(_createdDbConnection, _getLastCreatedId, _getEncryptionKey);
        }

    }
}
