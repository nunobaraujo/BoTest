using System;

namespace Contracts
{
    public interface IVat
    {
        string Id { get; }

        DateTime CreationDate { get; }
        string Description { get; }
        DateTime ExpirationDate { get; }
        
        string Name { get; }
        string TaxType { get; }
        decimal? TaxAmount { get; }
        string TaxCode { get; }
        string TaxCountryType { get; }
        decimal? TaxPercent { get; }
    }
}