using MessagePack;
using System;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class JobOptions : IJobOptions
    {
        public string Id { get; set; }
        public string JobOptionsCategoryId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Name { get; set; }
        public bool ShowOnMain { get; set; }
    }
}
