using Contracts.Api;

namespace Contracts.Clients
{
    public interface IRestClient
    {
        ISessionApi SessionApi { get; }
        IUserApi UserApi { get; }
        ICompanyApi CompanyApi { get; }

        IJobApi JobApi { get; }
    }
}
