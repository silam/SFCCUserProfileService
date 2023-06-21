using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SFCCUserProfileService.Models.UserProfile;
using SFCCUserProfileService.Models.UserProfile.Profiles;

namespace SFCCUserProfileService.Models.UserProfile
{
    public class Profile
    {
        public List<Address> addresses;
        public List<Email> emails;
        public List<Phone> phones;

        public string occupation;

        public Preferencess Preferencess;

        public IndustrialAccount industrial_account;

        public string home_store;

        [JsonPropertyName("ufx_information")]
        public UfxInformation ufx_information;

        //public List<RelatedAccount> related_accounts = new List<RelatedAccount>();

        public string profile_notes;

    }
}
