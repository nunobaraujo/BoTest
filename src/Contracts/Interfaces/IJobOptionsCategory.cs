using System;

namespace Contracts
{
    public interface IJobOptionsCategory
    {
        string Id { get; }

        DateTime CreationDate { get; }
        
        string Name { get; }
    }
}