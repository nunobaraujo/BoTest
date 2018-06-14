using Contracts.Models;
using System;

namespace Contracts.Requests
{
    [Serializable]
    public class JobRequest : BearerTokenRequest
    {
        public Job Job { get; set; }
    }
}
