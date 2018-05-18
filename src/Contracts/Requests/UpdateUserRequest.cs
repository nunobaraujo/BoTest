using Contracts.Models;

namespace Contracts.Requests
{
    public class UpdateUserRequest:BearerTokenRequest
    {
        public User User { get; set; }
    }
}
