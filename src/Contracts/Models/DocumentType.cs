using MessagePack;
using System;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class DocumentType : IDocumentType
    {
        public string Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string DocRef { get; set; }
        public string DocType { get; set; }
        public bool IsDebit { get; set; }
        public bool MoveStock { get; set; }
        public string Name { get; set; }
    }
}
