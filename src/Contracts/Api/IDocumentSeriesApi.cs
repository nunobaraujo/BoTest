using Contracts.Models;
using Contracts.Requests;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts.Api
{
    public interface IDocumentSeriesApi
    {
        /// <summary>
        /// List document type
        /// </summary>
        [Get("/api/documentseries/List/")]
        Task<List<DocumentSeries>> List([Body]BearerTokenRequest request);

        /// <summary>
        /// Create new document type
        /// </summary>
        [Post("/api/documentseries/")]
        Task<string> Add([Body]DocumentSeriesRequest request);

        /// <summary>
        /// Get document type by id
        /// </summary>
        [Get("/api/documentseries/{documentSeriesId}")]
        Task<DocumentType> Get(string documentSeriesId, BearerTokenRequest request);


        /// <summary>
        /// Update document type
        /// </summary>
        [Put("/api/documentseries/{documentSeriesId}")]
        Task<string> Update(string documentSeriesId, [Body]DocumentSeriesRequest request);

        /// <summary>
        /// Delete document type
        /// </summary>
        [Delete("/api/documentseries/{documentSeriesId}")]
        Task Delete(string documentSeriesId, [Body]BearerTokenRequest request);
    }
}
