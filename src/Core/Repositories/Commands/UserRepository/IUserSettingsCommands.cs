using Contracts;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.UserRepository
{
    public interface IUserSettingsCommands
    {
        Task<IUserSettings> Get(string userName);
        Task<string> Add(IUserSettings userSettings);
        Task<string> Update(IUserSettings userSettings);
        Task Delete(string userName);
    }
}
