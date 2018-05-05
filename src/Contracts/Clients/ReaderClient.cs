using Contracts.Infrastructure;
using Refit;
using System.Net.Http;

namespace Contracts.Clients
{
    internal class ReaderClient : IReaderClient
    {
        public ISessionApi SessionApi { get; }
        public IUserApi UserApi { get; }

        public ReaderClient(string url, string apiKey, string userAgent)
        {
            var httpMessageHandler = new ApiHeaderHttpClientHandler(new HttpClientHandler(), apiKey, userAgent);
            var settings = new RefitSettings { HttpMessageHandlerFactory = () => httpMessageHandler };
            SessionApi = RestService.For<ISessionApi>(url, settings);
            UserApi = RestService.For<IUserApi>(url, settings);
        }
    }
}
