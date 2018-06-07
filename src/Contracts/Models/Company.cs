using System;

namespace Contracts.Models
{
    public class Company : ICompany
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Reference { get; set; }
        public DateTime CreationDate { get; set; }
        public string TaxIdNumber { get; set; }
        public string MobilePhone { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string EMail { get; set; }
        public string URL { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CAE { get; set; }
        public string IBAN { get; set; }
        public bool Encrypted { get; set; }
    }
}
