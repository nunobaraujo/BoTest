﻿using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface IJobOptionsCommands : ICrudBaseCommands<IJobOptions>
    {
        Task<IList<IJobOptions>> GetByJob(string jobId);
    }
}
