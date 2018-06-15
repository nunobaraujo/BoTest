using System;

namespace Contracts
{
    public interface IJobService
    {
        string Id { get; }
        string JobId { get; }
        string ServiceId { get; }

        DateTime BeginDate { get; }
        DateTime CreationDate { get; }
        DateTime? FinishDate { get; }
        
        string Notes { get; }
        decimal PricePerMinute { get; }
        decimal Quantity { get; }
        int QuantityType { get; }        
        
        string Unit { get; }
        decimal UnitPrice { get; }
        string UserId { get; }
    }
}