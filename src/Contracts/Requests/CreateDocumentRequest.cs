using Contracts.Models;
using System.Collections.Generic;

namespace Contracts.Requests
{
    public  class CreateDocumentRequest:BearerTokenRequest
    {
        public Document Document { get; set; }
        public IList<DocumentLine> DocumentLines { get; set; }
    }
}
