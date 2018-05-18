using Contracts.Api;
using Contracts.Infrastructure;
using Refit;
using System.Net.Http;

namespace Contracts.Clients
{
    internal class RestClient : IRestClient
    {
        public ISessionApi SessionApi { get; }
        public IUserApi UserApi { get; }
        public ICompanyApi CompanyApi { get; }
        public IJobApi JobApi { get; }

        public RestClient(string url, string apiKey, string userAgent)
        {
            var httpMessageHandler = new ApiHeaderHttpClientHandler(new HttpClientHandler(), apiKey, userAgent);
            var settings = new RefitSettings { HttpMessageHandlerFactory = () => httpMessageHandler };
            SessionApi = RestService.For<ISessionApi>(url, settings);
            UserApi = RestService.For<IUserApi>(url, settings);
            CompanyApi = RestService.For<ICompanyApi>(url, settings);
            JobApi = RestService.For<IJobApi>(url, settings);
        }
    }
}
