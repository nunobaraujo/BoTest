using Contracts.Models;

namespace Contracts.Requests
{
    public class JobRequest : BearerTokenRequest
    {
        public Job Job { get; set; }
    }
}
