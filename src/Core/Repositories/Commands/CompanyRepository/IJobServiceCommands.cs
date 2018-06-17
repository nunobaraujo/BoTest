using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface IJobServiceCommands : ICrudBaseCommands<IJobService>
    {
        Task<IList<IJobService>> GetOngoing();
        Task<IList<IJobService>> GetByJob(string jobId);
    }
}
