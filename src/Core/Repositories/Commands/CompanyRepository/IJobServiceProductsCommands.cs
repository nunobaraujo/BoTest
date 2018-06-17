using Contracts;
using System.Collections.Generic;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface IJobServiceProductsCommands : ICrudBaseCommands<IJobServiceProducts>
    {
        IList<IJobServiceProducts> GetByJob(string jobId);
    }
}
