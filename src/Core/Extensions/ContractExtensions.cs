using Contracts.User;
using Contracts.User.Models;

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
                Id = src.Id,
                Language = src.Language,
                LastName = src.LastName,
                PasswordHash = src.PasswordHash,
                Pin = src.Pin,
                PostalCode = src.PostalCode,
                Theme = src.Theme,
                UserName = src.UserName
            };
        }
    }
}
