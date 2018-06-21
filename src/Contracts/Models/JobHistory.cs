using MessagePack;
using System;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class JobHistory : IJobHistory
    {
        public string Id { get; set; }
        public string JobId { get; set; }
        public string Comments { get; set; }
        public string JobReference { get; set; }
        public int JobState { get; set; }
        public DateTime StatusDate { get; set; }
        public string UserId { get; set; }
    }
}
