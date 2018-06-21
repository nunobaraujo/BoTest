using MessagePack;
using System;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class DocumentLine : IDocumentLine
    {
        public string DocumentId { get; set; }

        public string Id { get; set; }

        public string ProductId { get; set; }

        public string Comments { get; set; }

        public DateTime CreationDate { get; set; }

        public string Description { get; set; }

        public decimal Discount { get; set; }

        public string InternalRef { get; set; }

        public long LineNumber { get; set; }

        public string LineRef { get; set; }

        public decimal Quantity { get; set; }

        public string Unit { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Vat { get; set; }
    }
}
