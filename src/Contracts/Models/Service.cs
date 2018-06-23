using MessagePack;
using System;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Service : IService
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public bool AutoClose { get; set; }
        public DateTime CreationDate { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public decimal PricePerMinute { get; set; }
        public int QuantityType { get; set; }
        public string ServiceRef { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VatTax { get; set; }
    }
}
