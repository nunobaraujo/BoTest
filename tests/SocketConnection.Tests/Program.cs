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
        }

        static async Task RunTests()
        {
            IRestClient cli = new SocketClient.Client("localhost", 9865, null);
            
            var token = await cli.SessionApi.LogIn(new LogInRequest { UserName = "sa", UserPassword = "#Na123" });
            Console.WriteLine(token);

        }
    }
}
