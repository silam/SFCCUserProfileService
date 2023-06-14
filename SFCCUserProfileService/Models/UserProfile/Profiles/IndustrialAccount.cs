using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFCCUserProfileService.Models.UserProfile.Profiles
{
    public class IndustrialAccount
    {
        public string account_name { get; set; }
        public string program_name { get; set; }
        public string program_id { get; set; }
        public string last_transaction_date { get; set; }

        public string voucher_type { get; set; }

    }
}
