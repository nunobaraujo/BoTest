using System;

namespace Contracts
{
    public interface ITicket
    {
        string Id { get; }
        string CustomerId { get; }
        string CustomerRouteId { get; }
        string JobId { get; }

        DateTime CreationDate { get; }
        string Notes { get; }
        int Number { get; }
        string UserId { get; }
    }
}
