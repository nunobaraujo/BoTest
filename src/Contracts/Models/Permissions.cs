using MessagePack;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Permissions : IPermissions
    {
        public string Id { get; set; }
        public string CommandPath { get; set; }
        public int PermissionLevel { get; set; }
    }
}
