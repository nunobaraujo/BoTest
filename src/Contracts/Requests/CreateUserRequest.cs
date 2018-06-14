using System;

namespace Contracts.Requests
{
    [Serializable]
    public class CreateUserRequest: LogInRequest
    {
        public string Email { get; set; }
    }
}
