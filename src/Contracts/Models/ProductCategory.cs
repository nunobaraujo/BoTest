using MessagePack;
using System;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class ProductCategory : IProductCategory
    {
        public string Id { get; set; }
        public DateTime? IntegrationDate { get; set; }
        public string IntegrationRef { get; set; }
        public string Name { get; set; }
    }
}
