using System;

namespace Core.Services.License.Models
{
    public class License : ILicense
    {
        public DateTime CreationDate{ get; set; }
        public string Name{ get; set; }
        public string Address{ get; set; }
        public string PostalCode{ get; set; }
        public string City{ get; set; }
        public string Country{ get; set; }
        public string TaxIdNumber{ get; set; }
        public string Mobile{ get; set; }
        public string Phone{ get; set; }
        public string Fax{ get; set; }
        public string Email{ get; set; }
        public string Url{ get; set; }
        public int LicenseType{ get; set; }
        public bool IsRental{ get; set; }
        public DateTime LicenseDate{ get; set; }
        public DateTime LicenseExpiration{ get; set; }
        public string UIK{ get; set; }
        public Guid ServerId{ get; set; }
    }
}
