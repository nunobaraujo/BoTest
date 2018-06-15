using Contracts.Api;

namespace Contracts.Clients
{
    public interface IRestClient
    {
        ISessionApi SessionApi { get; }
        IUserApi UserApi { get; }
        ICompanyApi CompanyApi { get; }

        ICompanyOptionsApi CompanyOptionsApi { get; }
        ICustomerApi CustomerApi { get; }
        ICustomerRouteApi CustomerRouteApi { get; }
        IDocumentTypeApi DocumentTypeApi { get; }
        IJobApi JobApi { get; }
    }
}
