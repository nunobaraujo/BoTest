using System;

namespace Contracts
{
    public interface IProduct
    {
        string Id { get; }
        string ProductCategoryId { get; }

        DateTime CreationDate { get; }
        string Description { get; }        
        
        DateTime? IntegrationDate { get; }
        string IntegrationRef { get; }
        bool IsService { get; }
        string Name { get; }
        string Notes { get; }
        
        string ProductCode { get; }
        int ProductUnitType { get; }
        string Unit { get; }
        decimal UnitPrice { get; }
        decimal VatTax { get; }
    }
}