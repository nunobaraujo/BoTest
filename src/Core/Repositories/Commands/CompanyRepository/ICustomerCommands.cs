using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface ICustomerCommands: ICrudBaseCommands<ICustomer>
    {
        Task<IList<ICustomer>> List();
    }
}
