using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface ICustomerRouteCommands: ICrudBaseCommands<ICustomerRoute>
    {   
        Task<IList<ICustomerRoute>> GetByCustomer(string customerId);
    }
}
