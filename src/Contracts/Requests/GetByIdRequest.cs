namespace Contracts.Requests
{
    public class GetByIdRequest: AuthenticatedRequest
    {
        public string Id { get; set; }        
    }
}
