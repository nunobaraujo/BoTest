namespace Contracts
{
    public interface IPermissions
    {
        string Id { get; }

        string CommandPath { get; }        
        int PermissionLevel { get; }
    }
}