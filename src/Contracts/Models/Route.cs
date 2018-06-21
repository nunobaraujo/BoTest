using MessagePack;
using System;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Route : IRoute
    {
        public string Id { get; set; }

        public string Comments { get; set; }

        public DateTime CreationDate { get; set; }

        public int DayOfWeek { get; set; }

        public string Name { get; set; }
    }
}
