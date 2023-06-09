using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Database
{
    public class FederalLicenseModel
    {
        public int Id { get; set; }
        public int PlayerId {get; set; }
        public bool GpsFindLicense { get; set; }
        public bool PhoneHistoryLicense { get; set; }
        public bool SadLicense { get; set; }

        public FederalLicenseModel() { }
        public FederalLicenseModel(int playerId, bool gpsFindLicense, bool phoneHistoryLicense, bool sadLicense)
        {
            PlayerId = playerId;
            GpsFindLicense = gpsFindLicense;
            PhoneHistoryLicense = phoneHistoryLicense;
            SadLicense = sadLicense;
        }
    }
}
