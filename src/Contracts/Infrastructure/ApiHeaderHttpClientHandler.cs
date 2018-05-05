using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Contracts.Infrastructure
{
    internal class ApiHeaderHttpClientHandler : DelegatingHandler
    {
        private readonly string _apiKey;
        private readonly string _userAgent;

        public ApiHeaderHttpClientHandler(HttpMessageHandler innerHandler, string apiKey, string userAgent)
            : base(innerHandler)
        {
            _apiKey = apiKey;
            _userAgent = userAgent;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.UserAgent.Clear();
            request.Headers.TryAddWithoutValidation("User-Agent", _userAgent);
            request.Headers.TryAddWithoutValidation("api-key", _apiKey);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
