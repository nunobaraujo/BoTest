using MessagePack;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class CompanyOptions : ICompanyOptions
    {
        public string Id { get; set; }
        public string CloseJobReport { get; set; }
        public string IntegrationReturnDocument { get; set; }
        public string IntegrationShippingDocument { get; set; }
        public string IntegrationType { get; set; }
        public string JobFileReport { get; set; }
        public string JobTicketReport { get; set; }
        public string ReturnDocument { get; set; }
        public string ShippingDocument { get; set; }
    }
}
