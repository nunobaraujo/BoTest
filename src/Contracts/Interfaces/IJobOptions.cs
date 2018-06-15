using System;

namespace Contracts
{
    public interface IJobOptions
    {
        string Id { get; }
        string JobOptionsCategoryId { get; }

        DateTime CreationDate { get; }
        
        string Name { get; }
        bool ShowOnMain { get; }
    }
}