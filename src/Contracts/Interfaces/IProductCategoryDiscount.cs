namespace Contracts
{
    public interface IProductCategoryDiscount
    {
        string Id { get; }
        string ProductCategoryId { get; }
        string CustomerId { get; }

        string Comments { get; }        
        
        decimal DiscountPercent { get; }
        
    }
}