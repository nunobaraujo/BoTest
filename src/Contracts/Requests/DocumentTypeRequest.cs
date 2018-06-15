using Contracts.Models;
using System;

namespace Contracts.Requests
{
    [Serializable]
    public class DocumentTypeRequest : BearerTokenRequest
    {
        public DocumentType DocumentType { get; set; }
    }
}
