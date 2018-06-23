using MessagePack;
using System;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Ticket : ITicket
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerRouteId { get; set; }
        public string JobId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Notes { get; set; }
        public int Number { get; set; }
        public string UserId { get; set; }
    }
}
