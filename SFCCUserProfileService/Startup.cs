using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using SFCCUserProfileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[assembly: FunctionsStartup(typeof(Startup))]
namespace SFCCUserProfileService
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // TODO: Register dependencies here
        }
    }
}
