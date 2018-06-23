using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Contracts.Clients;
using System;
using Autofac.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Contracts.Requests;
using System.Linq;

namespace ApiClient.Tests
{
    internal static class Program
    {
        private static int _counter;

        static async Task Main(string[] args)
        {
            System.Threading.Thread.Sleep(5000);
            var services = new ServiceCollection();
            var builder = new ContainerBuilder();
            services.RegisterApiClient("http://localhost:5000", "botest", "TestClient");
            builder.Populate(services);
            var container = builder.Build();
            var apiClient = container.Resolve<IRestClient>();
            // 0
            var sessionToken = await apiClient.SessionApi.LogIn(new LogInRequest { UserName = "sa", UserPassword = "#Na123" }).Dump();
            // 1
            var userId = await apiClient.UserApi.Get(new IdRequest { Id = "1", Token = sessionToken }).Dump();

            //await JobTests(apiClient, sessionToken);
            await BusinessTests(apiClient, sessionToken);

            // Last
            await apiClient.SessionApi.LogOut(new BearerTokenRequest { Token = sessionToken }).Dump();

            Console.WriteLine("All Tests Finished");
            Console.ReadKey();
        }

        private static async Task JobTests(IRestClient apiClient, string sessionToken)
        {
            var btoken = new BearerTokenRequest { Token = sessionToken };
            var jobs = await apiClient.JobApi.GetByDate(new DateIntervalRequest { DateFrom = DateTime.Parse("2010-01-01"), DateTo = DateTime.UtcNow.AddDays(1), Token = sessionToken }).Dump();
            var lastJob = await apiClient.JobApi.Get(jobs.Where(x => x.CustomerRouteId != null).Last().Id, btoken).Dump();

            var job = new Contracts.Models.Job
            {
                Id = Guid.NewGuid().ToString(),
                CustomerId = lastJob.CustomerId,
                CustomerRouteId = lastJob.CustomerRouteId,
                ProductId = lastJob.ProductId,
                UserId = lastJob.UserId,
                CreationDate = DateTime.UtcNow,
                BeginDate = DateTime.UtcNow,
                Description ="test job",
                CurrentState =  0,
                ClientRef = "cref",
                JobReference =  "TestJobRef",
                Notes = "New Notes"
            };
            var newJob = await apiClient.JobApi.Add(new JobRequest { Token = sessionToken, Job = job }).Dump();
            var savedJob = await apiClient.JobApi.Get(newJob, btoken);
            savedJob.Notes = "Edited Notes";
            await apiClient.JobApi.Update(savedJob.Id, new JobRequest { Token = sessionToken, Job = savedJob }).Dump();
            var savedJob2 = await apiClient.JobApi.Get(newJob, btoken).Dump();
            if (savedJob2.Notes != savedJob.Notes)
                Console.WriteLine("Update JObe Failed");

                        
            var clientList = await apiClient.JobApi.GetByCustomer(savedJob2.CustomerId, btoken).Dump();
            var clientRouteList = await apiClient.JobApi.GetByCustomerRoute(savedJob2.CustomerRouteId, btoken).Dump();
            await apiClient.JobApi.Delete(savedJob2.Id, btoken).Dump();

        }

        private static async Task BusinessTests(IRestClient apiClient, string sessionToken)
        {
            var clientList = await apiClient.CustomerApi.List(new BearerTokenRequest { Token = sessionToken });
            var seriesList = await apiClient.DocumentSeriesApi.List(new BearerTokenRequest { Token = sessionToken });
            //var productList = await apiClient.ProductApi.List(new BearerTokenRequest { Token = sessionToken });

            var btoken = new CreateDocumentRequest { Token = sessionToken };
            btoken.Document = new Contracts.Models.Document
            {
                Id = Guid.NewGuid().ToString().Replace("-", "").ToUpper(),
                CustomerId = clientList.Last().Id,
                DocumentSeriesId = seriesList.Last().Id,
                CreationDate = DateTime.UtcNow,
                Coin = "EUR",
                DocumentDate = DateTime.UtcNow,
                CustomerRouteId = null,               
                Description = "Description ",
                Exchange = 1,
                ExpirationDate = DateTime.UtcNow.AddDays(1),
                Notes ="",
                PaymentType ="PP",
                Report =""
            };

            var result = await apiClient.BusinessApi.CreateDocument(btoken);


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
