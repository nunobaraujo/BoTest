using System;

namespace Contracts.Requests
{
    public class GetByDateRequest: BearerTokenRequest
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
