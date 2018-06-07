using Core.Repositories;
using Core.Repositories.Commands.UserRepository;
using NBsoft.Logs.Interfaces;
using Repositories.Common;
using System.Data;
using System.Data.SqlClient;

namespace Repositories.Sql
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connString;
        private string _encryptionKey;

        public IUserCommands User { get; }
        public ICompanyCommands Company { get; }
        public IUserSettingsCommands UserSettings { get; }
        public ICompanyUserCommands CompanyUser { get; }

        public UserRepository(string connString, ILogger log)
        {
            _connString = connString;
            _encryptionKey = Core.Constants.NbSoftKey;

            var userRepositoryFactory = new UserRepositoryFactory(() => new SqlConnection(_connString), () => Constants.GetLastInsertedId , () => _encryptionKey, log);
            User = userRepositoryFactory.CreateUserCommands();
            Company = userRepositoryFactory.CreateCompanyCommands();
            UserSettings = userRepositoryFactory.CreateUserSettingsCommands();
            CompanyUser = userRepositoryFactory.CreateCompanyUserCommands();
        }
        
        public void SetEncryptionKey(string encryptionKey)
        {
            _encryptionKey = encryptionKey;
        }
    }
}
