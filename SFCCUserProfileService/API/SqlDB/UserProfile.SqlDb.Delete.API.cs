using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using SFCCUserProfileService.Models.UserProfile;
using Azure.Storage.Blobs;
using AutoNumber;
using SFCCUserProfileService.Models.UserProfile.Profiles;
using System.Text.Json.Serialization;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace SFCCUserProfileService.API.SqlDB
{
    public class DeleteSqlDbUserProfile
    {
        [FunctionName("DeleteSqlDbUserProfile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "SqlDb/UserProfiles")] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            var newconfiguration = new ConfigurationBuilder()
                                    .SetBasePath(context.FunctionAppDirectory)
                                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                    .AddJsonFile("localSecrets.settings.json", optional: true, reloadOnChange: true)
                                    .AddEnvironmentVariables()
                                    .Build();
            var SqlDBConnectionString = "Server=tcp:dev-nexgen-db-sql.database.windows.net,1433;Initial Catalog=UP-POC-DB;Persist Security Info=False;User ID=AzureSA;Password=dbsPd&1ovx&84U;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                        
            if (req.Method == "DELETE")
            {
                string email = req.Query["email"];
                using (SqlConnection conn = new SqlConnection(SqlDBConnectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("dbo.[DeleteUserProfileByEmail]", conn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@email", email));

                    // execute the command
                    int ret = cmd.ExecuteNonQuery();

                    return new OkObjectResult("Return = " + ret);
                }

            }
            else
            {
                return new OkObjectResult(null);

            }

        }
    }
}
