using System;

namespace Contracts
{
    public interface ICompany
    {
        string Id { get; }
        string Name { get; }
        string Reference { get; }
        DateTime CreationDate { get; }
        string TaxIdNumber { get; }
        string MobilePhone { get; }
        string Telephone { get; }
        string Fax { get; }
        string EMail { get; }
        string URL { get; }
        string Address { get; }
        string PostalCode { get; }
        string City { get; }
        string Country { get; }
        string CAE { get; }
        string IBAN { get; }
    }
}
