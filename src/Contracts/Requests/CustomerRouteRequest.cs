using Contracts.Models;
using System;

namespace Contracts.Requests
{
    [Serializable]
    public class CustomerRouteRequest : BearerTokenRequest
    {
        public CustomerRoute CustomerRoute { get; set; }
    }
}
