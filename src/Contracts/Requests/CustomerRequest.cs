using Contracts.Models;
using System;

namespace Contracts.Requests
{
    [Serializable]
    public class CustomerRequest : BearerTokenRequest
    {
        public Customer Customer { get; set; }
    }
}
