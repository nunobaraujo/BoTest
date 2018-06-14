using System;

namespace Contracts.Requests
{
    [Serializable]
    public class IdRequest: BearerTokenRequest
    {
        public string Id { get; set; }        
    }
}
