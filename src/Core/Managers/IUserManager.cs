using Contracts;
using Core.Repositories;
using Core.Services.License;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Managers
{
    public interface IUserManager
    {
        Task<string> LogIn(string userName, string password, string userInfo);
        Task LogOut(string sessionToken);

        Task<IUser> CreateUser(string userName, string password, string email);
                
        Task<IUser> GetUserById(string sessionToken, string userId);
        Task<IUser> UpdateUser(string sessionToken, IUser user);
        Task DeleteUser(string sessionToken, string userId);

        Task SetPassword(string sessionToken, string password);

        Task<List<ICompanyUser>> GetCompanies(string sessionToken);

        Task<IUserRepository> GetUserRepository(string sessionToken);
        Task<ICompanyRepository> ResolveRepository(string sessionToken);

        Task<ICompany> GetActiveCompany(string sessionToken);
        Task<ICompany> SetActiveCompany(string sessionToken, string companyId);

        Task<ILicense> GetLicense(string sessionToken);
        Task SetLicense(string sessionToken, ILicense license);
    }
}
