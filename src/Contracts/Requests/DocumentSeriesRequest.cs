using Contracts.Models;
using System;

namespace Contracts.Requests
{
    [Serializable]
    public class DocumentSeriesRequest:BearerTokenRequest
    {
        public DocumentSeries DocumentSeries { get; set; }
    }
}
