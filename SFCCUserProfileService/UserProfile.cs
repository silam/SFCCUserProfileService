using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFCCUserProfileService
{
    public class UserProfile
    {
        public string id { get; set; }
        public string first_name { get; set; }

        public string last_name { get; set; }

        public string email { get; set; }

        public string billing_address { get; set; }

        public string billing_zipcode { get; set; }

        public string billing_state { get; set; }



    }
}
