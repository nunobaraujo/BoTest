using Core.Services.Session;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.SessionRepository
{
    public interface ISessionCommands
    {
        Task<IUserSession> Get(string token);
        Task New(IUserSession session);
        Task Update(IUserSession session);
        Task Remove(string token);
    }
}
