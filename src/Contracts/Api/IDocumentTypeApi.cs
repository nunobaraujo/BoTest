using Contracts.Models;
using Contracts.Requests;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Api
{
    public interface IDocumentTypeApi
    {
        /// <summary>
        /// List document type
        /// </summary>
        [Get("/api/documenttype/List/")]
        Task<List<DocumentType>> List([Body]BearerTokenRequest request);

        /// <summary>
        /// Create new document type
        /// </summary>
        [Post("/api/documenttype/")]
        Task<string> Add([Body]DocumentTypeRequest request);

        /// <summary>
        /// Get document type by id
        /// </summary>
        [Get("/api/documenttype/{documentTypeId}")]
        Task<DocumentType> Get(string documentTypeId, BearerTokenRequest request);


        /// <summary>
        /// Update document type
        /// </summary>
        [Put("/api/documenttype/{documentTypeId}")]
        Task<string> Update(string documentTypeId, [Body]DocumentTypeRequest request);

        /// <summary>
        /// Delete document type
        /// </summary>
        [Delete("/api/documenttype/{documentTypeId}")]
        Task Delete(string documentTypeId, [Body]BearerTokenRequest request);
    }
}
