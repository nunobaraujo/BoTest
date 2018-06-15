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
                Salt = src.Salt,
                Encrypted = src.Encrypted
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
                URL = src.URL,
                Encrypted = src.Encrypted
            };
        }

        public static Job ToDto(this IJob src)
        {
            return new Job
            {
                BeginDate = src.BeginDate,
                ClientRef = src.ClientRef,
                CreationDate = src.CreationDate,
                CurrentState = src.CurrentState,
                CustomerId = src.CustomerId,
                CustomerRouteId = src.CustomerRouteId,
                Description = src.Description,
                FinishDate = src.FinishDate,
                Id = src.Id,
                JobReference = src.JobReference,
                Notes = src.Notes,
                Option1 = src.Option1,
                Option2 = src.Option2,
                Option3 = src.Option3,
                Option4 = src.Option4,
                Option5 = src.Option5,
                Option6 = src.Option6,
                ProductHeight = src.ProductHeight,
                ProductId = src.ProductId,
                ProductLength = src.ProductLength,
                ProductUnitType = src.ProductUnitType,
                ProductWidth = src.ProductWidth,
                TotalValue = src.TotalValue,
                UserId = src.UserId
            };
        }

        public static CompanyUser ToDto(this ICompanyUser src)
        {
            return new CompanyUser
            {
                CompanyId = src.CompanyId,
                Id = src.Id,
                IsDefault = src.IsDefault,
                PermissionLevel = src.PermissionLevel,
                UserName = src.UserName
            };
        }
    }
}
