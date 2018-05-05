namespace Contracts.User
{
    public interface ICompanyUser
    {
        string Id { get; }
        string CompanyId { get; }
        string UserId { get; }
        short PermissionLevel { get; }
        bool IsDefault { get; }
    }
}
