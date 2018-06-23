using Core.Repositories.Commands.CompanyRepository;
using NBsoft.Logs.Interfaces;
using Repositories.Common.Commands.CompanyRepository;
using Repositories.Common.Commands.UserRepository;
using System;
using System.Data;

namespace Repositories.Common
{
    public class CompanyRepositoryFactory
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly Func<string> _getLastCreatedId;
        private readonly Func<string> _getEncryptionKey;
        private readonly ILogger _log;

        public CompanyRepositoryFactory(Func<IDbConnection> createdDbConnection, Func<string> getLastCreatedId, Func<string> getEncryptionKey, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _getLastCreatedId = getLastCreatedId;
            _getEncryptionKey = getEncryptionKey;
            _log = log;
        }

        public ICompanyOptionsCommands CreateCompanyOptionsCommands() => new CompanyOptionsCommands(_createdDbConnection, _log);
        public ICustomerCommands CreateCustomerCommands() => new CustomerCommands(_createdDbConnection, _log);
        public ICustomerRouteCommands CreateCustomerRouteCommands() => new CustomerRouteCommands(_createdDbConnection, _log);
        public IDocumentCommands CreateDocumentCommands() => new DocumentCommands(_createdDbConnection, _log);
        public IDocumentLineCommands CreateDocumentLineCommands() => new DocumentLineCommands(_createdDbConnection, _log);

        public IDocumentSeriesCommands CreateDocumentSeriesCommands() => new DocumentSeriesCommands(_createdDbConnection, _log);
        public IDocumentTypeCommands CreateDocumentTypeCommands() => new DocumentTypeCommands(_createdDbConnection, _log);
        public IFrontTerminalCommands CreateFrontTerminalCommands() => new FrontTerminalCommands(_createdDbConnection, _log);
        public IJobCommands CreateJobCommands() => new JobCommands(_createdDbConnection, _log);
        public IJobHistoryCommands CreateJobHistoryCommands() => new JobHistoryCommands(_createdDbConnection, _log);

        public IJobOptionsCommands CreateJobOptionsCommands() => new JobOptionsCommands(_createdDbConnection, _log);
        public IJobOptionsJobCommands CreateJobOptionsJobCommands() => new JobOptionsJobCommands(_createdDbConnection, _log);
        public IJobServiceCommands CreateJobServiceCommands() => new JobServiceCommands(_createdDbConnection, _log);
        public IJobServiceProductsCommands CreateJobServiceProductsCommands() => new JobServiceProductsCommands(_createdDbConnection, _log);
        public IPaymentDueCommands CreatePaymentDueCommands() => new PaymentDueCommands(_createdDbConnection, _log);

        public IPermissionsCommands CreatePermissionsCommands() => new PermissionsCommands(_createdDbConnection, _log);
        public IProductCategoryCommands CreateProductCategoryCommands() => new ProductCategoryCommands(_createdDbConnection, _log);
        public IProductCategoryDiscountCommands CreateProductCategoryDiscountCommands() => new ProductCategoryDiscountCommands(_createdDbConnection, _log);
        public IProductCommands CreateProductCommands() => new ProductCommands(_createdDbConnection, _log);
        public IRouteCommands CreateRouteCommands() => new RouteCommands(_createdDbConnection, _log);

        public IServiceCommands CreateServiceCommands() => new ServiceCommands(_createdDbConnection, _log);
        public ITicketCommands CreateTicketCommands() => new TicketCommands(_createdDbConnection, _log);
        public IVatCommands CreateVatCommands() => new VatCommands(_createdDbConnection, _log);

    }
}
