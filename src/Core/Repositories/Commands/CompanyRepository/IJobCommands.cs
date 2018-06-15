using Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface IJobCommands: ICrudBaseCommands<IJob>
    {
        Task<IList<IJob>> GetByDate(DateTime from, DateTime to);
        Task<IList<IJob>> GetByCustomer(string customerId);
        Task<IList<IJob>> GetByCustomerRoute(string customerRouteId);
    }
}
