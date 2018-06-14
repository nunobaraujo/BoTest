using System;

namespace Contracts.Requests
{
    [Serializable]
    public class ChangePasswordRequest: BearerTokenRequest
    {
        public string NewPassword { get; set; }
    }
}
