using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface IJobOptionsJobCommands : ICrudBaseCommands<IJobOptionsJob>
    {
        Task<IList<IJobOptionsJob>> GetByJob(string jobId);
    }
}
