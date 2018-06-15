using System;

namespace Contracts
{
    public interface ICustomerRoute
    {
        string Id { get; }
        string CustomerId { get; }        
        string RouteId { get; }

        string Address { get; }
        string City { get; }
        string Comments { get; }
        string Country { get; }
        DateTime CreationDate { get; }        
        string Name { get; }
        string PostalCode { get; }
    }
}