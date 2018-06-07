using Contracts.Models;

namespace Contracts.Requests
{
    public class UserRequest:BearerTokenRequest
    {
        public User User { get; set; }
    }
}
