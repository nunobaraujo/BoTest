using Contracts.Models;

namespace Contracts.Requests
{
    public class CustomerRequest : BearerTokenRequest
    {
        public Customer Customer { get; set; }
    }
}
