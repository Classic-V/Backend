using Backend.Services.FederalLicense.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.FederalLicense
{
    public class FederalLicenseService : IFederalLicenseService
    {
        public List<FederalLicenseModel> FederalLicenses { get; private set; }

        public FederalLicenseService()
        {
            var ctx = new DBContext();
            FederalLicenses = new List<FederalLicenseModel>(ctx.FederalLicenses);
        }

        public async Task AddLicense(FederalLicenseModel model)
        {
            FederalLicenses.Add(model);

            var ctx = new DBContext();
            ctx.FederalLicenses.Add(model);
            await ctx.SaveChangesAsync();
        }

        public Task<FederalLicenseModel?> GetLicense(int id)
        {
            return Task.FromResult(FederalLicenses.FirstOrDefault(x => x.PlayerId == id));
        }

        public async Task RemoveLicense(FederalLicenseModel model)
        {
            FederalLicenses.Remove(model);

            var ctx = new DBContext();
            ctx.FederalLicenses.Remove(model);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateLicense(FederalLicenseModel model)
        {
            var ctx = new DBContext();
            ctx.FederalLicenses.Update(model);
            await ctx.SaveChangesAsync();
        }
    }
}
