using System;

namespace Contracts
{
    public interface IDocumentLine
    {
        string DocumentId { get; }
        string Id { get; }
        string ProductId { get; }

        string Comments { get; }
        DateTime CreationDate { get; }
        string Description { get; }
        decimal Discount { get; }        
        
        string InternalRef { get; }
        long LineNumber { get; }
        string LineRef { get; }        
        
        decimal Quantity { get; }
        string Unit { get; }
        decimal UnitPrice { get; }
        decimal Vat { get; }
    }
}