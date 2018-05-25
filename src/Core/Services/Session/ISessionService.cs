using System.Threading.Tasks;

namespace Core.Services.Session
{
    public interface ISessionService
    {
        Task<string> CreateNewSession(string userId, string userInfo);
        Task<string> GetUserId(string sessionToken);
        Task KillSession(string sessionToken);
        Task<IUserSession> GetSession(string sessionToken);
        Task UpdateSessionUserInfo(string sessionToken, string userInfo);
    }
}
