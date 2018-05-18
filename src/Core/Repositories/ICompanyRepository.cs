using Core.Repositories.Commands.CompanyRepository;

namespace Core.Repositories
{
    public interface ICompanyRepository
    {
        void SetEncryptionKey(string encryptionKey);

        IJobCommands Job { get; }
    }
}
