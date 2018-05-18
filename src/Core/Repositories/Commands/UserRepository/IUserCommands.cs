using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.UserRepository
{
    public interface IUserCommands
    {
        Task<IUser> Get(string userName);
        Task<IUser> GetByEmail(string email);

        Task<IEnumerable<ICompanyUser>> GetCompanies(string userId);
        Task<string> Auth(string userName, string userPassword);
        Task SetPassword(string userId, string password);
        IUser GeneratePassword(IUser user, string password);

        Task<string> Add(IUser user);
        Task<string> Update(IUser user);
        Task Delete(string userName);
    }
}
