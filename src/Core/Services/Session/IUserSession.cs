using System;

namespace Core.Services.Session
{
    public interface IUserSession
    {
        string UserId { get; }
        string SessionToken { get; }
        string UserInfo { get; }
        string ActiveCompany { get; }
        DateTime Registered { get; }
        DateTime LastAction { get; }
    }
}
