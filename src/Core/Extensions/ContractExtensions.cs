using Contracts;
using Contracts.Models;

namespace Core.Extensions
{
    public static class ContractExtensions
    {
        public static User ToDto(this IUser src)
        {
            return new User()
            {
                Accentcolor = src.Accentcolor,
                Address = src.Address,
                City = src.City,
                Country = src.Country,
                CreationDate = src.CreationDate,
                Email = src.Email,
                FirstName = src.FirstName,
                Language = src.Language,
                LastName = src.LastName,
                PasswordHash = src.PasswordHash,
                Pin = src.Pin,
                PostalCode = src.PostalCode,
                Theme = src.Theme,
                UserName = src.UserName,
                Salt = src.Salt
            };
        }

        public static Company ToDto(this ICompany src)
        {
            return new Company()
            {   
                Address = src.Address,
                City = src.City,
                Country = src.Country,
                CreationDate = src.CreationDate,
                PostalCode = src.PostalCode,
                CAE = src.CAE,
                EMail = src.EMail,
                Fax = src.Fax,
                IBAN = src.IBAN,
                Id = src.Id,
                MobilePhone = src.MobilePhone,
                Name = src.Name,
                Reference = src.Reference,
                TaxIdNumber = src.TaxIdNumber,
                Telephone = src.Telephone,
                URL = src.URL
            };
        }
    }
}
