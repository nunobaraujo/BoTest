using System;

namespace Contracts
{
    public interface IService
    {
        string Id { get; }
        string ProductId { get; }

        bool AutoClose { get; }
        DateTime CreationDate { get; }
        string Description { get; }
        
        string Name { get; }
        string Notes { get; }
        decimal PricePerMinute { get; }        
        
        int QuantityType { get; }
        string ServiceRef { get; }
        decimal UnitPrice { get; }
        decimal VatTax { get; }
    }
}