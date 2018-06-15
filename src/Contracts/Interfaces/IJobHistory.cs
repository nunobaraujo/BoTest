using System;

namespace Contracts
{
    public interface IJobHistory
    {
        string Id { get; }
        string JobId { get; }

        string Comments { get; }
        
        string JobReference { get; }
        int JobState { get; }
        DateTime StatusDate { get; }
        string UserId { get; }
    }
}