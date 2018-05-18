using Contracts.Models;

namespace Contracts.Requests
{
    public class CompanyRequest : BearerTokenRequest
    {
        public Company Company { get; set; }
    }
}
