using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface IDocumentLineCommands : ICrudBaseCommands<IDocumentLine>
    {
        Task<IList<IDocumentLine>> GetByDocument(string docId);
    }
}
