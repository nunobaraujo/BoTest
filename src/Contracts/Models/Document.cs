using MessagePack;
using System;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Document : IDocument
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerRouteId { get; set; }
        public string DocumentSeriesId { get; set; }
        public string Coin { get; set; }
        public DateTime CreationDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryCountry { get; set; }
        public string DeliveryPostalCode { get; set; }
        public string Description { get; set; }
        public string DocRef { get; set; }
        public DateTime DocumentDate { get; set; }
        public decimal Exchange { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal ClientDiscount { get; set; }
        public decimal FinanceDiscount { get; set; }
        public decimal FinanceDiscountVal { get; set; }
        public decimal Iec { get; set; }
        public DateTime? IntegrationDate { get; set; }
        public string IntegrationRef { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public long Number { get; set; }
        public string PaymentType { get; set; }
        public string Report { get; set; }
        public decimal TotalProducts { get; set; }
        public string UserId { get; set; }
        public decimal Vatvalue { get; set; }

        public string GenerateDocRef()
        {
            throw new NotImplementedException();
        }

        public string GenerateDocName()
        {
            throw new NotImplementedException();
        }

        public string GenerateDocDescription()
        {
            throw new NotImplementedException();
        }
    }
}
