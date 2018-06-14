using Contracts.Models;
using System;

namespace Contracts.Requests
{
    [Serializable]
    public class UserRequest:BearerTokenRequest
    {
        public User User { get; set; }
    }
}
