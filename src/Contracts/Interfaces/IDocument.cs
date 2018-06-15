using System;

namespace Contracts
{
    public interface IDocument
    {
        string Id { get; }
        string CustomerId { get; }
        string CustomerRouteId { get; }
        string DocumentSeriesId { get; }
                
        string Coin { get; }
        DateTime CreationDate { get; }

        string DeliveryAddress { get; }
        string DeliveryCity { get; }
        string DeliveryCountry { get; }
        string DeliveryPostalCode { get; }
        string Description { get; }
        string DocRef { get; }
        DateTime DocumentDate { get; }

        decimal Exchange { get; }
        DateTime ExpirationDate { get; }
        decimal ClientDiscount { get; }
        decimal FinanceDiscount { get; }
        decimal FinanceDiscountVal { get; }

        decimal Iec { get; }
        DateTime? IntegrationDate { get; }
        string IntegrationRef { get; }
        string Name { get; }
        string Notes { get; }
        long Number { get; }
        string PaymentType { get; }
        string Report { get; }
        decimal TotalProducts { get; }
        string UserId { get; }
        decimal Vatvalue { get; }

    }
}
