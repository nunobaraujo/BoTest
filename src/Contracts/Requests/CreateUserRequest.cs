namespace Contracts.Requests
{
    public class CreateUserRequest: LogInRequest
    {
        public string Email { get; set; }
    }
}
