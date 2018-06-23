using MessagePack;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class ProductCategoryDiscount : IProductCategoryDiscount
    {
        public string Id { get; set; }
        public string ProductCategoryId { get; set; }
        public string CustomerId { get; set; }
        public string Comments { get; set; }
        public decimal DiscountPercent { get; set; }
    }
}
