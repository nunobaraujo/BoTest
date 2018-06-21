using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface IJobServiceProductsCommands : ICrudBaseCommands<IJobServiceProducts>
    {
        Task<IList<IJobServiceProducts>> GetByJob(string jobId);
    }
}
