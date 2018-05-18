using Contracts;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface IJobCommands
    {
        Task<IJob> Get(string jobId);
        Task<string> Add(IJob job);
        Task<string> Update(IJob job);
        Task Delete(string jobId);
    }
}
