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
    public class GetSqlDbUserProfile
    {
        [FunctionName("GetSqlDbUserProfile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "SqlDb/UserProfiles")] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            var newconfiguration = new ConfigurationBuilder()
                                    .SetBasePath(context.FunctionAppDirectory)
                                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                    .AddJsonFile("localSecrets.settings.json", optional: true, reloadOnChange: true)
                                    .AddEnvironmentVariables()
                                    .Build();
            var SqlDBConnectionString = "Server=tcp:dev-nexgen-db-sql.database.windows.net,1433;Initial Catalog=UP-POC-DB;Persist Security Info=False;User ID=AzureSA;Password=dbsPd&1ovx&84U;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


            if (req.Method == "GET")
            {
                try
                {
                    string id = req.Query["person_key"];
                    string email = req.Query["email"];
                    string firstName = req.Query["firstName"];
                    string lastName = req.Query["lastName"];
                    string phone = req.Query["phone"];

                    List<OutUserProfile> profiles = new List<OutUserProfile>();


                    using (SqlConnection conn = new SqlConnection(SqlDBConnectionString))
                    {
                        conn.Open();

                        if (firstName != null)
                        {
                            if (firstName == string.Empty)
                            {
                                return new ContentResult()
                                {
                                    Content = "first name is empty",
                                    ContentType = "appliation/json",
                                    StatusCode = 400

                                };
                            }
                            else
                            {
                                SqlCommand cmd = new SqlCommand("dbo.GetUserProfileByFirstName", conn);

                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add(new SqlParameter("@fname", firstName));

                                // execute the command
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    // iterate through results, printing each to console
                                    while (reader.Read())
                                    {
                                        var user = new OutUserProfile();
                                        user.record_id = reader["record_id"].ToString();
                                        user.first_name = reader["first_name"].ToString();

                                        user.last_name = reader["last_name"].ToString();
                                        user.person_key = reader["person_key"].ToString();
                                        user.id = reader["id"].ToString();

                                        dynamic json = JsonConvert.DeserializeObject(reader["profile"].ToString());

                                        user.profile = json;

                                        profiles.Add(user);
                                    }
                                }
                            }

                        }
                        else if (lastName != null)
                        {
                            if (lastName == string.Empty)
                            {
                                return new ContentResult()
                                {
                                    Content = "lastName is empty",
                                    ContentType = "appliation/json",
                                    StatusCode = 400

                                };
                            }
                            else
                            {
                                SqlCommand cmd = new SqlCommand("dbo.GetUserProfileByLastName", conn);

                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add(new SqlParameter("@lname", lastName));

                                // execute the command
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    // iterate through results, printing each to console
                                    while (reader.Read())
                                    {
                                        var user = new OutUserProfile();
                                        user.id = reader["id"].ToString();
                                        user.record_id = reader["record_id"].ToString();
                                        user.first_name = reader["first_name"].ToString();

                                        user.last_name = reader["last_name"].ToString();
                                        user.person_key = reader["person_key"].ToString();

                                        dynamic json = JsonConvert.DeserializeObject(reader["profile"].ToString());

                                        user.profile = json;

                                        profiles.Add(user);
                                    }
                                }
                            }

                        }
                        else if (email != null)
                        {
                            if (email == string.Empty)
                            {
                                return new ContentResult()
                                {
                                    Content = "first name is empty",
                                    ContentType = "appliation/json",
                                    StatusCode = 400

                                };
                            }
                            else
                            {
                                SqlCommand cmd = new SqlCommand("dbo.GetUserProfileByEmail", conn);

                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.Add(new SqlParameter("@email", email));

                                // execute the command
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    // iterate through results, printing each to console
                                    while (reader.Read())
                                    {
                                        var user = new OutUserProfile();
                                        user.record_id = reader["record_id"].ToString();
                                        user.first_name = reader["first_name"].ToString();

                                        user.last_name = reader["last_name"].ToString();
                                        user.person_key = reader["person_key"].ToString();
                                        user.id = reader["id"].ToString();


                                        dynamic json = JsonConvert.DeserializeObject(reader["profile"].ToString());

                                        user.profile = json;

                                        profiles.Add(user);
                                    }
                                }
                            }

                        }

                    }


                    
                    return new OkObjectResult(profiles);
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
            else
            {
                return new OkObjectResult(null);

            }

        }
    }
}
