using Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface IDocumentCommands : ICrudBaseCommands<IDocument>
    {
        Task<IDocument> Get(string seriesId, long docNumber);
        Task<string> GetName(string id);
        Task<string> GetDescription(string id);

        Task<IList<IDocument>> GetByCustomer(string customerId);
        Task<IList<IDocument>> GetByDate(DateTime dateFrom, DateTime dateTo);
        Task<IList<IDocument>> GetBySeries(string seriesId);
    }
}
