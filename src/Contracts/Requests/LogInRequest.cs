using System;

namespace Contracts.Requests
{
    [Serializable]
    public class LogInRequest
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
    }
}
