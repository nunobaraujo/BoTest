using MessagePack;
using System;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class DocumentSeries : IDocumentSeries
    {
        public string Id { get; set; }
        public string DocumentTypeId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Description { get; set; }
        public long LastNumber { get; set; }
        public string Name { get; set; }
        public string Report { get; set; }
        public bool IsDefault { get; set; }
    }
}
