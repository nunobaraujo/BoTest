using Contracts.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public interface IUserRepository
    {
        Task<IUser> GetUser(string userId);
        Task<IUser> GetUserByUserName(string userName);
        Task<ICompany> GetCompany(string companyId);
        Task<IEnumerable<ICompanyUser>> GetCompanyByUser(string userId);
        Task<string> AuthUser(string userName, string userPassword);

        Task AddUser(IUser user);
        Task UpdateUser(IUser user);
        Task DeleteUser(IUser user);

        Task AddCompany(ICompany company);
        Task UpdateCompany(ICompany company);
        Task DeleteCompany(ICompany company);

        Task AddCompanyUser(ICompanyUser companyUser);
        Task UpdateCompanyUser(ICompanyUser companyUser);
        Task DeleteCompanyUser(ICompanyUser companyUser);

    }
}
