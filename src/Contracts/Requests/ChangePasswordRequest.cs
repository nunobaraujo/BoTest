namespace Contracts.Requests
{
    public class ChangePasswordRequest: BearerTokenRequest
    {
        public string NewPassword { get; set; }
    }
}
