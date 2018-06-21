using MessagePack;
using System;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class JobOptionsCategory : IJobOptionsCategory
    {
        public string Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
    }
}
