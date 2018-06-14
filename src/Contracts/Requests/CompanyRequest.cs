using Contracts.Models;
using System;

namespace Contracts.Requests
{
    [Serializable]
    public class CompanyRequest : BearerTokenRequest
    {
        public Company Company { get; set; }
    }
}
