using Core.Repositories.Commands.CompanyRepository;

namespace Core.Repositories
{
    public interface ICompanyRepository
    {
        void SetEncryptionKey(string encryptionKey);
                
        ICompanyOptionsCommands CompanyOptions { get; }
        ICustomerCommands Customer { get; }
        ICustomerRouteCommands CustomerRoute { get; }
        IDocumentSeriesCommands DocumentSeries { get; }
        IDocumentTypeCommands DocumentType{ get; }
        IJobCommands Job { get; }
        IFrontTerminalCommands FrontTerminal { get; }
        IJobHistoryCommands JobHistory { get; }
        IJobOptionsCommands JobOptions { get; }
        IJobOptionsCategoryCommands JobOptionsCategory { get; }
        IJobOptionsJobCommands JobOptionsJob { get; }
        IJobServiceCommands JobService { get; }

    }
}
