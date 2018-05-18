using Microsoft.Extensions.DependencyInjection;

namespace Contracts.Clients
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterApiClient(this IServiceCollection services, string url, string key, string userAgent)
        {
            services.AddSingleton<IRestClient>(p => new RestClient(url, key, userAgent));
        }
    }
}
