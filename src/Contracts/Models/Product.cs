using MessagePack;
using System;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Product : IProduct
    {
        public string Id { get; set; }

        public string ProductCategoryId { get; set; }

        public DateTime CreationDate { get; set; }

        public string Description { get; set; }

        public DateTime? IntegrationDate { get; set; }

        public string IntegrationRef { get; set; }

        public bool IsService { get; set; }

        public string Name { get; set; }

        public string Notes { get; set; }

        public string ProductCode { get; set; }

        public int ProductUnitType { get; set; }

        public string Unit { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal VatTax { get; set; }
    }
}
