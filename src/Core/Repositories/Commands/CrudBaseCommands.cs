using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands
{
    public interface ICrudBaseCommands<T>
    {
        Task<T> Get(string Id);
        Task<string> Add(T model);
        Task<string> Update(T model);
        Task Delete(string jobId);
    }
}
