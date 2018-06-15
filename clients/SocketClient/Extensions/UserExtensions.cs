using Contracts;
using Contracts.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Comms.Sockets;
using Contracts.Models;
using Core.Extensions;

namespace SocketClient.Extensions
{
    static class UserExtensions
    {
        public static Task<User> UserAdd(this Client cli, CreateUserRequest request)
        {
            var result = cli.SendCustomMessage(SubCommand.UserAdd, request);
            return Task.FromResult(result.GetParameter<User>());
        }
        public static Task UserChangePassword(this Client cli, ChangePasswordRequest request)
        {
            var result = cli.SendCustomMessage(SubCommand.UserChangePassword, request);
            return Task.FromResult(0);
        }

        public static Task UserDelete(this Client cli, IdRequest request)
        {
            var result = cli.SendCustomMessage(SubCommand.UserDelete, request);
            return Task.FromResult(0);
        }

        public static Task<User> UserGet(this Client cli, IdRequest request)
        {
            var result = cli.SendCustomMessage(SubCommand.UserGet, request);
            return Task.FromResult(result.GetParameter<User>());
        }

        public static Task<List<ICompanyUser>> UserGetCompanies(this Client cli, BearerTokenRequest request)
        {
            var result = cli.SendCustomMessage(SubCommand.UserGetCompanies, request);
            return Task.FromResult(result.GetParameter<List<ICompanyUser>>());
        }

        public static Task<User> UserUpdate(this Client cli, UserRequest request)
        {
            var result = cli.SendCustomMessage(SubCommand.UserUpdate, request);
            return Task.FromResult(result.GetParameter<User>());
        }

    }
}
