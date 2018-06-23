using MessagePack;
using System;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class JobService : IJobService
    {
        public string Id { get; set; }

        public string JobId { get; set; }

        public string ServiceId { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? FinishDate { get; set; }

        public string Notes { get; set; }

        public decimal PricePerMinute { get; set; }

        public decimal Quantity { get; set; }

        public int QuantityType { get; set; }

        public string Unit { get; set; }

        public decimal UnitPrice { get; set; }

        public string UserId { get; set; }
    }
}
