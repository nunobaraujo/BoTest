using MessagePack;

namespace Contracts.Models
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class UserSettings : IUserSettings
    {
        public string UserName { get; set; }
        public string LastOpenCompanyId { get; set; }
    }
}
