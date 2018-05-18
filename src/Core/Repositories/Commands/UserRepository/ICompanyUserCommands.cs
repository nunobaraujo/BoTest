using Contracts;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.UserRepository
{
    public interface ICompanyUserCommands
    {
        Task<string> Add(ICompanyUser companyUser);
        Task<string> Update(ICompanyUser companyUser);
        Task Delete(string companyUserId);
    }
}
