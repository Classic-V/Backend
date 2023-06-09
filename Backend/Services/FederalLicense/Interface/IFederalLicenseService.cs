using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.FederalLicense.Interface
{
    public interface IFederalLicenseService
    {
        List<FederalLicenseModel> FederalLicenses { get; }

        Task AddLicense(FederalLicenseModel model);
        Task RemoveLicense(FederalLicenseModel model);
        Task UpdateLicense(FederalLicenseModel model);
        Task<FederalLicenseModel?> GetLicense(int id);
    }
}
