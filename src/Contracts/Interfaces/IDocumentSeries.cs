using System;

namespace Contracts
{
    public interface IDocumentSeries
    {
        string Id { get; }
        string DocumentTypeId { get; }
        
        DateTime CreationDate { get; }
        string Description { get; }        
        long LastNumber { get; }
        string Name { get; }
        string Report { get; }
        bool IsDefault { get; }
    }
}