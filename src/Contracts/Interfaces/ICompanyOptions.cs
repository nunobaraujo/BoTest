namespace Contracts
{
    public interface ICompanyOptions
    {
        string Id { get; }
        string CloseJobReport { get; }
        string IntegrationReturnDocument { get; }
        string IntegrationShippingDocument { get; }
        string IntegrationType { get; }
        string JobFileReport { get; }
        string JobTicketReport { get; }
        string ReturnDocument { get; }
        string ShippingDocument { get; }
    }
}
