using System;

namespace Core.Services.License
{
    public interface ILicense
    {
        DateTime CreationDate { get; }
        string Name { get; }
        string Address { get; }
        string PostalCode { get; }
        string City { get; }
        string Country { get; }
        string TaxIdNumber { get; }
        string Mobile { get; }
        string Phone { get; }
        string Fax { get; }
        string Email { get; }
        string Url { get; }

        int LicenseType { get; }
        bool IsRental { get; }
        DateTime LicenseDate { get; }
        DateTime LicenseExpiration { get; }

        string UIK { get; }
        Guid ServerId { get; }
    }
}
