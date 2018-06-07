using Core.Services.License;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class LicenseService : ILicenseService
    {
        public Task<ILicense> GetLicense(string companyId)
        {
            throw new NotImplementedException();
        }

        public Task SetLicense(ILicense license)
        {
            throw new NotImplementedException();
        }
    }
}
