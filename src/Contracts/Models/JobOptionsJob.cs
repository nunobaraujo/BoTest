using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class JobOptionsJob : IJobOptionsJob
    {
        public string Id { get; set; }
        public string JobId { get; set; }
        public string JobOptionsId { get; set; }
    }
}
