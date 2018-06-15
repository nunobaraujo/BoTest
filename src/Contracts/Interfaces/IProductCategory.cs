using System;

namespace Contracts
{
    public interface IProductCategory
    {
        string Id { get; }

        DateTime? IntegrationDate { get; }
        string IntegrationRef { get; }
        string Name { get; }        
    }
}