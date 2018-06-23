using Contracts.Models;
using Contracts.Requests;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Api
{   
    public interface IBusinessApi
    {
        /// <summary>
        /// List document type
        /// </summary>
        [Post("/api/business/document/create")]
        Task<Document> CreateDocument([Body]CreateDocumentRequest request);
    }
}
