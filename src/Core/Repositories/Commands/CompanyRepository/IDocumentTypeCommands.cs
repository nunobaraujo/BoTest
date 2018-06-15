using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface IDocumentTypeCommands: ICrudBaseCommands<IDocumentType>
    {
        Task<IList<IDocumentType>> List();
    }
}
