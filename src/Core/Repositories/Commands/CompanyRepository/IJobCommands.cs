using Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface IJobCommands
    {
        Task<IJob> Get(string jobId);
        Task<IList<IJob>> List();
        Task<string> Add(IJob job);
        Task<string> Update(IJob job);
        Task Delete(string jobId);

        Task<IList<IJob>> GetByDate(DateTime from, DateTime to);
        Task<IList<IJob>> GetByCustomer(string customerId);
        Task<IList<IJob>> GetByCustomerRoute(string customerRouteId);

    }
}
