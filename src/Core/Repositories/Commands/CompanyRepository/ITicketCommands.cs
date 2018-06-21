using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface ITicketCommands : ICrudBaseCommands<ITicket>
    {
        Task<IList<ITicket>> GetByCustomer(string customerId);
        Task<IList<ITicket>> GetByCustomerRoute(string customerRouteId);
        Task<IList<ITicket>> GetByJob(string jobId);
    }
}
