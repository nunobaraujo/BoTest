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

        public IJobCommands CreateJobCommands()
        {
            return new JobCommands(_createdDbConnection,  _log);
        }
        public ICompanyOptionsCommands CreateCompanyOptionsCommands()
        {
            return new CompanyOptionsCommands(_createdDbConnection, _log);
        }
        public ICustomerCommands CreateCustomerCommands()
        {
            return new CustomerCommands(_createdDbConnection, _log);
        }
        public ICustomerRouteCommands CreateCustomerRouteCommands()
        {
            return new CustomerRouteCommands(_createdDbConnection, _log);
        }
        public IDocumentCommands CreateDocumentCommands()
        {
            return new DocumentCommands(_createdDbConnection, _log);
        }
        public IDocumentLineCommands CreateDocumentLineCommands()
        {
            return new DocumentLineCommands(_createdDbConnection, _log);
        }
        public IDocumentSeriesCommands CreateDocumentSeriesCommands()
        {
            return new DocumentSeriesCommands(_createdDbConnection, _log);
        }
        public IRouteCommands CreateRouteCommands()
        {
            return new RouteCommands(_createdDbConnection, _log);
        }
    }
}
