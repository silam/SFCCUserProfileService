using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFCCUserProfileService.Models.UserProfile
{
    public class OutUserProfile
    {
        public string id { get; set; }
        public string record_id { get; set; }
        public string person_key { get; set; }
        public string first_name { get; set; }

        public string last_name { get; set; }

        public bool? rwsc_employee { get; set; } = true;

        public dynamic profile { get; set; }

    }
}
