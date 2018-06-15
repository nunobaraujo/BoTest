using Contracts;
using Contracts.Models;
using Core.Repositories;
using Core.Services.License;
using Core.Services.License.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Managers
{
    public interface IUserManager
    {
        Task<string> LogIn(string userName, string password, string userInfo);
        Task LogOut(string sessionToken);

        Task<User> CreateUser(string userName, string password, string email);
                
        Task<User> GetUserById(string sessionToken, string userId);
        Task<User> UpdateUser(string sessionToken, User user);
        Task DeleteUser(string sessionToken, string userId);

        Task SetPassword(string sessionToken, string password);

        Task<List<CompanyUser>> GetCompanies(string sessionToken);

        Task<IUserRepository> GetUserRepository(string sessionToken);
        Task<ICompanyRepository> ResolveRepository(string sessionToken);

        Task<Company> GetActiveCompany(string sessionToken);
        Task<Company> SetActiveCompany(string sessionToken, string companyId);

        Task<License> GetLicense(string sessionToken);
        Task SetLicense(string sessionToken, License license);
    }
}
