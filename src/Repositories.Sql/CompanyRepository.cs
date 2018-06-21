using Core.Repositories;
using Core.Repositories.Commands.CompanyRepository;
using NBsoft.Logs.Interfaces;
using Repositories.Common;
using System.Data.SqlClient;

namespace Repositories.Sql
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly string _connString;
        private string _encryptionKey;

        public IJobCommands Job { get; }
        public ICompanyOptionsCommands CompanyOptions { get; }
        public ICustomerCommands Customer { get; }
        public ICustomerRouteCommands CustomerRoute { get; }
        public IDocumentSeriesCommands DocumentSeries { get; }
        public IDocumentTypeCommands DocumentType { get; }
        public IFrontTerminalCommands FrontTerminal { get; }
        public IJobHistoryCommands JobHistory { get; }
        public IJobOptionsCommands JobOptions { get; }
        public IJobOptionsCategoryCommands JobOptionsCategory { get; }
        public IDocumentCommands Document { get; }
        public IDocumentLineCommands DocumentLine { get; }
        public IJobOptionsJobCommands JobOptionsJob { get; }
        public IJobServiceCommands JobService { get; }
        public IJobServiceProductsCommands JobServiceProducts { get; }
        public IPaymentDueCommands PaymentDue { get; }
        public IPermissionsCommands Permissions { get; }
        public IProductCategoryCommands ProductCategory { get; }
        public IProductCategoryDiscountCommands ProductCategoryDiscount { get; }
        public IProductCommands Product { get; }
        public IRouteCommands Route { get; }
        public IServiceCommands Service { get; }
        public ITicketCommands Ticket { get; }
        public IVatCommands Vat { get; }

        public CompanyRepository(string connString, ILogger log)
        {
            _connString = connString;
            _encryptionKey = Core.Constants.NbSoftKey;

            var companyRepositoryFactory = new CompanyRepositoryFactory(() => new SqlConnection(_connString), () => Constants.GetLastInsertedId, () => _encryptionKey, log);
                        
            CompanyOptions = companyRepositoryFactory.CreateCompanyOptionsCommands();
            Customer = companyRepositoryFactory.CreateCustomerCommands();
            CustomerRoute = companyRepositoryFactory.CreateCustomerRouteCommands();
            Document = companyRepositoryFactory.CreateDocumentCommands();
            DocumentLine = companyRepositoryFactory.CreateDocumentLineCommands();
            DocumentSeries = companyRepositoryFactory.CreateDocumentSeriesCommands();
            DocumentType = companyRepositoryFactory.CreateDocumentTypeCommands();
            FrontTerminal = companyRepositoryFactory.CreateFrontTerminalCommands();
            Job = companyRepositoryFactory.CreateJobCommands();

            Route = companyRepositoryFactory.CreateRouteCommands();
        }

        public void SetEncryptionKey(string encryptionKey)
        {
            _encryptionKey = encryptionKey;
        }
    }
}
