using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Contracts.Clients;
using System;
using Autofac.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Contracts.Requests;

namespace ApiClient.Tests
{
    internal static class Program
    {
        private static int _counter;

        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            var builder = new ContainerBuilder();
            services.RegisterApiClient("http://localhost:5000", "botest", "TestClient");
            builder.Populate(services);
            var container = builder.Build();
            var apiClient = container.Resolve<IRestClient>();
            // 0
            var sessionToken = await apiClient.SessionApi.LogIn(new LogInRequest { UserName = "sa", UserPassword = "na123456" }).Dump();
            // 1
            var userId = await apiClient.UserApi.GetUserById(new GetByIdRequest { Id = "1", Token = sessionToken }).Dump();
            // 2
            await apiClient.SessionApi.LogOut(new BearerTokenRequest { Token = sessionToken }).Dump();

            Console.WriteLine("All Tests Finished");
        }

        public static T Dump<T>(this T o)
        {
            var str = o is string s ? s : JsonConvert.SerializeObject(o);
            Console.WriteLine("\n\r\n\r{0}. {1}", ++_counter, str);
            return o;
        }

        public static async Task<T> Dump<T>(this Task<T> t)
        {
            return (await t).Dump();
        }

        public static async Task Dump(this Task o)
        {
            await o;
            "ok".Dump();
        }
    }
}
