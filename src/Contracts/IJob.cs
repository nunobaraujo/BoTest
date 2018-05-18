using System;

namespace Contracts
{
    public interface IJob
    {
        string Id { get; }
        string CustomerId { get; }
        string CustomerRouteId { get; }
        string ProductId { get; }

        DateTime BeginDate { get; }
        string ClientRef { get; }
        DateTime CreationDate { get; }
        int CurrentState { get; }

        string Description { get; }
        DateTime? FinishDate { get; }

        string JobReference { get; }
        string Notes { get; }
        bool Option1 { get; }
        bool Option2 { get; }
        bool Option3 { get; }
        bool Option4 { get; }
        bool Option5 { get; }
        bool Option6 { get; }
        decimal ProductHeight { get; }

        decimal ProductLength { get; }
        int ProductUnitType { get; }
        decimal ProductWidth { get; }
        decimal TotalValue { get; }
        string UserId { get; }
    }
}
