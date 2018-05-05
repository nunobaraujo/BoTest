using Microsoft.Extensions.DependencyInjection;

namespace Contracts.Clients
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterApiReaderClient(this IServiceCollection services, string url, string key, string userAgent)
        {
            services.AddSingleton<IReaderClient>(p => new ReaderClient(url, key, userAgent));
        }
    }
}
