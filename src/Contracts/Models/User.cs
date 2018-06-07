using Newtonsoft.Json;
using System;

namespace Contracts.Models
{
    public class User : IUser
    {   
        public string UserName { get; set; }
        [JsonIgnore]
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreationDate { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Theme { get; set; }
        public int? Accentcolor { get; set; }
        public string Language { get; set; }
        public string Pin { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string Salt { get; set; }
        public bool Encrypted { get; set; }
    }
}
