using Contracts.Requests;
using System.Threading.Tasks;

namespace SocketClient.Extensions
{
    static class SessionExtensions
    {
        public static Task<string> SessionLogIn(this Client cli, LogInRequest request)
        {
            var result = cli.SendCustomMessage(Services.Comms.Sockets.SubCommand.SessionLogIn, request);
            return Task.FromResult(result.GetParameter<string>());
        }
        public static Task SessionLogOut(this Client cli, BearerTokenRequest request)
        {
            var result = cli.SendCustomMessage(Services.Comms.Sockets.SubCommand.SessionLogOut, request);
            return Task.FromResult(result.GetParameter<string>());
        }

        public static Task<string> SessionActiveCompanyGet(this Client cli, BearerTokenRequest request)
        {
            var result = cli.SendCustomMessage(Services.Comms.Sockets.SubCommand.SessionActiveCompanyGet, request);
            return Task.FromResult(result.GetParameter<string>());
        }

        public static Task SessionActiveCompanySet(this Client cli, IdRequest request)
        {
            var result = cli.SendCustomMessage(Services.Comms.Sockets.SubCommand.SessionActiveCompanySet, request);
            return Task.FromResult(0);
        }
    }
}
