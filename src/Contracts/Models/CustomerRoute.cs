using MessagePack;
using System;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class CustomerRoute : ICustomerRoute
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string RouteId { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Comments { get; set; }
        public string Country { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public string PostalCode { get; set; }
    }
}
