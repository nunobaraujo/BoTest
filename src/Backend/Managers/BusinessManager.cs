using Contracts;
using Contracts.Requests;
using Core.Extensions;
using Core.Managers;
using Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Managers
{
    public class BusinessManager : IBusinessManager
    {        
        private readonly ICompanyRepositoryResolver _companyRepositoryResolver;

        public BusinessManager(ICompanyRepositoryResolver companyRepositoryResolver)
        {
            _companyRepositoryResolver = companyRepositoryResolver;
        }

        public async Task<string> CreateDocument(string companyId, IDocument document, IList<IDocumentLine> documentLines)
        {
            var repo = _companyRepositoryResolver.Resolve(companyId);

            var customer = await repo.Customer.Get(document.CustomerId);
            var documentSeries = await repo.DocumentSeries.Get(document.DocumentSeriesId);
            var documentType = await repo.DocumentType.Get(documentSeries.DocumentTypeId);
            


            var newId = await repo.Document.Add(document);
            var newLines = documentLines.Select(x => x.ToDto());
            foreach (var item in newLines)
            {
                item.DocumentId = newId;
            }
            var LineIds = await repo.DocumentLine.AddBatch(newLines.Cast<IDocumentLine>().ToList());
            return newId;
        }
    }
}
