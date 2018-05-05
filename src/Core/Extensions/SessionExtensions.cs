using Core.Services.Session;
using Core.Services.Session.Models;

namespace Core.Extensions
{
    public static class SessionExtensions
    {
        public static UserSession ToDto(this IUserSession src)
        {
            return new UserSession
            {
                LastAction = src.LastAction,
                Registered = src.Registered,
                SessionToken = src.SessionToken,
                UserId = src.UserId,
                UserInfo = src.UserInfo
            };
        }
    }
}
