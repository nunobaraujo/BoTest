using Contracts;
using Contracts.Requests;
using Core.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Managers
{
    public interface IBusinessManager
    {
        Task<string> CreateDocument(string companyId, IDocument document, IList<IDocumentLine> documentLines);
    }
}
