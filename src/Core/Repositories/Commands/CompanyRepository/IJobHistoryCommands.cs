using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface IJobHistoryCommands : ICrudBaseCommands<IJobHistory>
    {
        Task<IList<IJobHistory>> GetByJob(string jobId);
    }
}
