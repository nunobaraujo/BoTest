namespace Contracts
{
    public interface ICompanyUser
    {
        string Id { get; }
        string CompanyId { get; }
        string UserName { get; }
        short PermissionLevel { get; }
        bool IsDefault { get; }
    }
}
