using Contracts;
using Core.Repositories.Commands.UserRepository;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Repositories.Common.Commands.UserRepository
{
    internal class UserSettingsCommands : IUserSettingsCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly Func<string> _getLastCreatedId;
        private readonly Func<string> _getEncryptionKey;

        public UserSettingsCommands(Func<IDbConnection> createdDbConnection, Func<string> getLastCreatedId, Func<string> getEncryptionKey)
        {
            _createdDbConnection = createdDbConnection;
            _getLastCreatedId = getLastCreatedId;
            _getEncryptionKey = getEncryptionKey;
        }

        public Task<string> Add(IUserSettings userSettings)
        {
            throw new NotImplementedException();
        }

        public Task Delete(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<IUserSettings> Get(string userName)
        {
            throw new NotImplementedException();
        }

        public Task<string> Update(IUserSettings userSettings)
        {
            throw new NotImplementedException();
        }
    }
}
