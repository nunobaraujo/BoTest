using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface IDocumentSeriesCommands : ICrudBaseCommands<IDocumentSeries>
    {
        Task<IList<IDocumentSeries>> GetByDocumentType(string documentTypeId);
    }
}
