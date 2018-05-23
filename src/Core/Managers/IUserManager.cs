using Contracts;
using Core.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Managers
{
    public interface IUserManager
    {
        Task<string> LogIn(string userName, string password, string userInfo);
        Task LogOut(string sessionToken);

        Task<string> CreateUser(string userName, string password, string email);
                
        Task<IUser> GetUserById(string sessionToken, string userId);
        Task<string> UpdateUser(string sessionToken, IUser user);
        Task DeleteUser(string sessionToken, string userId);

        Task SetPassword(string sessionToken, string password);

        Task<List<ICompanyUser>> GetCompanies(string sessionToken);

        Task<ICompanyRepository> ResolveRepository(string sessionToken);
    }
}
