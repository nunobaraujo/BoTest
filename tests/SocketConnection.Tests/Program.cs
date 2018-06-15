using Contracts.Clients;
using Contracts.Requests;
using System;
using System.Threading.Tasks;

namespace SocketConnection.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Threading.Thread.Sleep(5000);
            Task.Run(async () => await RunTests()).Wait();
            Console.WriteLine("Tests Finished");
            Console.ReadKey();
        }

        static async Task RunTests()
        {
            IRestClient cli = new SocketClient.Client("nem", 9865, null);

            var token = await cli.SessionApi.LogIn(new LogInRequest { UserName = "sa", UserPassword = "#Na123" });
            Console.WriteLine(token);
            //await RunSessionTests(cli, token);
            await RunUserTests(cli, token);

            await cli.SessionApi.LogOut(new BearerTokenRequest { Token = token });
        }

        private static async Task RunSessionTests(IRestClient cli, string token)
        {
            var companyId = await cli.SessionApi.GetActiveCompany(new BearerTokenRequest { Token = token });
            Console.WriteLine("{0} GetActiveCompany 1 = {1}", nameof(RunSessionTests), companyId);
            await cli.SessionApi.SetActiveCompany(new IdRequest { Token = token, Id = "3" });
            var newCompanyId = await cli.SessionApi.GetActiveCompany(new BearerTokenRequest { Token = token });
            Console.WriteLine("{0} GetActiveCompany 2 = {1}", nameof(RunSessionTests), newCompanyId);
            await cli.SessionApi.SetActiveCompany(new IdRequest { Token = token, Id = companyId });
        }

        private static async Task RunUserTests(IRestClient cli, string token)
        {
            var testUser = await cli.UserApi.Add(new CreateUserRequest
            {
                Email = "Test@test.com",
                UserName = Guid.NewGuid().ToString().Replace("-", ""),
                UserPassword = "SomePass"
            });
            Console.WriteLine("{0} Added Userr = {1}", nameof(RunUserTests), testUser.UserName);
        }
    }
}
