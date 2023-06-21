using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFCCUserProfileService.Models.UserProfile
{
    public class UserProfile
    {
        public string id { get; set; }
        public string record_id { get; set; }
        public string person_key { get; set; }
        public string first_name { get; set; }

        public string last_name { get; set; }

        public bool rwsc_employee { get; set; }

        public Profile profile { get; set; }


    }
}
