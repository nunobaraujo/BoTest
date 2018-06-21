using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface IRouteCommands : ICrudBaseCommands<IRoute>
    {
        Task<IList<IRoute>> List();
    }
}
