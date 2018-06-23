using MessagePack;
using System;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Vat : IVat
    {
        public string Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Description { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Name { get; set; }
        public string TaxType { get; set; }
        public decimal? TaxAmount { get; set; }
        public string TaxCode { get; set; }
        public string TaxCountryType { get; set; }
        public decimal? TaxPercent { get; set; }
    }
}
