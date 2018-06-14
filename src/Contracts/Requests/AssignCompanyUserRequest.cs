using System;

namespace Contracts.Requests
{
    [Serializable]
    public class AssignCompanyUserRequest:BearerTokenRequest
    {
        string CompanyId { get; set; }
        string UserId { get; set; }
        short PermissionLevel { get; set; }
        bool IsDefault { get; set; }
    }
}
