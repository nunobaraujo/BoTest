using System;

namespace Contracts.Models
{
    public class Customer : ICustomer
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ComercialName { get; set; }
        public string Comments { get; set; }
        public string Contact { get; set; }
        public string Country { get; set; }
        public DateTime CreationDate { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Iban { get; set; }
        public DateTime? IntegrationDate { get; set; }
        public string IntegrationRef { get; set; }
        public bool IsClient { get; set; }
        public bool IsSupplier { get; set; }
        public string MobilePhone { get; set; }
        public string Name { get; set; }
        public string PostalCode { get; set; }
        public string TaxIdNumber { get; set; }
        public string Telephone { get; set; }
        public string Url { get; set; }
    }
}
