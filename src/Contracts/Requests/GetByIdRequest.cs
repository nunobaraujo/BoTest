namespace Contracts.Requests
{
    public class GetByIdRequest: BearerTokenRequest
    {
        public string Id { get; set; }        
    }
}
