namespace Contracts.Models
{
    public class CompanyUser : ICompanyUser
    {
        public string Id { get; set; }
        public string CompanyId { get; set; }
        public string UserName { get; set; }
        public short PermissionLevel { get; set; }
        public bool IsDefault { get; set; }
    }
}
