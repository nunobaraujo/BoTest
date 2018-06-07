using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.UserRepository
{
    public interface ICompanyCommands
    {
        Task<IEnumerable<ICompany>> List();
        Task<ICompany> Get(string companyId);
        Task<string> Add(ICompany company);
        Task<string> Update(ICompany company);
        Task Delete(string companyId);

        Task<IEnumerable<string>> UpdateBatch(IEnumerable<ICompany> companies);
    }
}
