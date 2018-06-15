using Contracts.Models;

namespace Contracts.Requests
{
    public class CompanyOptionsRequest : BearerTokenRequest
    {
        public CompanyOptions Company { get; set; }
    }
}
