using System;

namespace Contracts
{
    public interface IJobServiceProducts
    {
        string Id { get; }
        string JobServiceId { get; }
        string ProductId { get; }
        
        DateTime CreationDate { get; }
        
        string Notes { get; }
        decimal PricePerUnit { get; }
        
        decimal Quantity { get; }
        string UserId { get; }
    }
}