using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands
{
    public interface ICrudBaseCommands<T>
    {
        Task<T> Get(string id);
        Task<string> Add(T model);
        Task<string> Update(T model);
        Task Delete(string id);

        Task<IList<string>> AddBatch(IList<T> models);
        Task<IList<string>> UpdateBatch(IList<T> models);
        Task DeleteBatch(IList<string> ids);
    }
}
