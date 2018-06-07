using System;

namespace Contracts
{
    public interface ICustomer
    {
        string Id { get; }

        string Address { get; }
        string City { get; }
        string ComercialName { get; }
        string Comments { get; }
        string Contact { get; }
        string Country { get; }
        DateTime CreationDate { get; }

        string Email { get; }
        string Fax { get; }
        string Iban { get; }

        DateTime? IntegrationDate { get; }
        string IntegrationRef { get; }
        bool IsClient { get; }
        bool IsSupplier { get; }

        string MobilePhone { get; }
        string Name { get; }
        string PostalCode { get; }
        string TaxIdNumber { get; }
        string Telephone { get; }

        string Url { get; }

    }
}
