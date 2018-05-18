using Core.Repositories.Commands.UserRepository;

namespace Core.Repositories
{
    public interface IUserRepository
    {
        void SetEncryptionKey(string encryptionKey);

        IUserCommands User { get; }
        ICompanyCommands Company { get; }
        IUserSettingsCommands UserSettings { get; }
        ICompanyUserCommands CompanyUser { get; }
    }
}
