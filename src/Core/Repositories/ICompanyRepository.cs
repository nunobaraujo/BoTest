using Core.Repositories.Commands.CompanyRepository;

namespace Core.Repositories
{
    public interface ICompanyRepository
    {
        void SetEncryptionKey(string encryptionKey);
                
        ICompanyOptionsCommands CompanyOptions { get; }
        ICustomerCommands Customer { get; }
        ICustomerRouteCommands CustomerRoute { get; }
        IDocumentCommands Document { get; }
        IDocumentLineCommands DocumentLine { get; }
        IDocumentSeriesCommands DocumentSeries { get; }
        IDocumentTypeCommands DocumentType{ get; }
        IJobCommands Job { get; }
        IFrontTerminalCommands FrontTerminal { get; }
        IJobHistoryCommands JobHistory { get; }
        IJobOptionsCommands JobOptions { get; }
        IJobOptionsCategoryCommands JobOptionsCategory { get; }
        IJobOptionsJobCommands JobOptionsJob { get; }
        IJobServiceCommands JobService { get; }
        IJobServiceProductsCommands JobServiceProducts { get; }
        IPaymentDueCommands PaymentDue { get; }
        IPermissionsCommands Permissions { get; }
        IProductCategoryCommands ProductCategory{ get; }
        IProductCategoryDiscountCommands ProductCategoryDiscount { get; }
        IProductCommands Product { get; }
        IRouteCommands Route { get; }
        IServiceCommands Service { get; }
        ITicketCommands Ticket { get; }
        IVatCommands Vat { get; }

    }
}
