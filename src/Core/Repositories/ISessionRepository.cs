using Core.Services.Session;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public interface ISessionRepository
    {
        Task<IUserSession> GetSession(string token);
        Task NewSession(IUserSession session);
        Task UpdateSession(IUserSession session);
        Task RemoveSession(string token);
    }
}
