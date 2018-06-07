using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.UserRepository
{
    public interface IUserCommands
    {
        Task<IEnumerable<IUser>> List();

        Task<IUser> Get(string userName);
        Task<IUser> GetByEmail(string email);

        Task<IEnumerable<ICompanyUser>> GetCompanies(string userId);
        Task<string> Auth(string userName, string userPassword);
        Task SetPassword(string userId, string password);
        IUser GeneratePassword(IUser user, string password);

        Task<IUser> Add(IUser user);
        Task<IUser> Update(IUser user);
        Task Delete(string userName);

        Task<IEnumerable<string>> UpdateBatch(IEnumerable<IUser> users);
    }
}
