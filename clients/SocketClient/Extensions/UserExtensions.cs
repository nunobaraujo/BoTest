using Contracts;
using Contracts.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocketClient.Extensions
{
    static class UserExtensions
    {
        public static Task<IUser> UserAdd(this Client cli, CreateUserRequest request)
        {
            var result = cli.SendCustomMessage(Services.Comms.Sockets.SubCommand.UserAdd, 0x00, request);
            return Task.FromResult(result.GetParameter<IUser>());
        }
        public static Task UserChangePassword(this Client cli, ChangePasswordRequest request)
        {
            var result = cli.SendCustomMessage(Services.Comms.Sockets.SubCommand.UserChangePassword, 0x00, request);
            return Task.FromResult(0);
        }

        public static Task UserDelete(this Client cli, IdRequest request)
        {
            var result = cli.SendCustomMessage(Services.Comms.Sockets.SubCommand.UserDelete, 0x00, request);
            return Task.FromResult(0);
        }

        public static Task<IUser> UserGet(this Client cli, IdRequest request)
        {
            var result = cli.SendCustomMessage(Services.Comms.Sockets.SubCommand.UserGet, 0x00, request);
            return Task.FromResult(result.GetParameter<IUser>());
        }

        public static Task<List<ICompanyUser>> UserGetCompanies(this Client cli, BearerTokenRequest request)
        {
            var result = cli.SendCustomMessage(Services.Comms.Sockets.SubCommand.UserGetCompanies, 0x00, request);
            return Task.FromResult(result.GetParameter<List<ICompanyUser>>());
        }

        public static Task<IUser> UserUpdate(this Client cli, UserRequest request)
        {
            var result = cli.SendCustomMessage(Services.Comms.Sockets.SubCommand.UserUpdate, 0x00, request);
            return Task.FromResult(result.GetParameter<IUser>());
        }

    }
}
