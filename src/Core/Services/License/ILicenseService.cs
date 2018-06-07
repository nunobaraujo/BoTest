using System.Threading.Tasks;

namespace Core.Services.License
{
    public interface ILicenseService
    {
        Task<ILicense> GetLicense(string companyId);
        Task SetLicense(ILicense license);
    }
}
