using System;

namespace Contracts
{
    public interface IDocumentType
    {
        string Id { get; }

        DateTime CreationDate { get; }
        string DocRef { get; }
        string DocType { get; }
                
        bool IsDebit { get; }
        bool MoveStock { get; }
        string Name { get; }
    }
}