using MessagePack;
using System;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class JobServiceProducts : IJobServiceProducts
    {
        public string Id { get; set; }
        public string JobServiceId { get; set; }
        public string ProductId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Notes { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal Quantity { get; set; }
        public string UserId { get; set; }
    }
}
