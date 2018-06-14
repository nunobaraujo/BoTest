using System;

namespace Contracts.Requests
{
    [Serializable]
    public class BearerTokenRequest
    {
        public string Token { get; set; }
    }
}
