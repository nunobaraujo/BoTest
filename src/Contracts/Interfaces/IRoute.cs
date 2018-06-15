using System;

namespace Contracts
{
    public interface IRoute
    {
        string Id { get; }

        string Comments { get; }
        DateTime CreationDate { get; }        
        int DayOfWeek { get; }
        
        string Name { get; }
    }
}