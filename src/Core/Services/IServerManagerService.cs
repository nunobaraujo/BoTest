using Contracts.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IServerManagerService
    {
        Task<string> LogIn(string userName, string password, string userInfo);
        Task LogOut(string sessionToken);
        Task<IUser> GetUserById(string sessionToken, string userId);
    }
}
