using Contracts;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface ICompanyOptionsCommands
    {
        Task<ICompanyOptions> Get();
        Task Set(ICompanyOptions companyOptions);
    }
}
