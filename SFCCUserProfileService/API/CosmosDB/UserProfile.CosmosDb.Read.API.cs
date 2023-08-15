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
using System.Diagnostics;

namespace SFCCUserProfileService.API.CosmosDB
{
    public class GetCosmosDbUserProfile
    {
        [FunctionName("GetCosmosDbUserProfile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "CosmosDb/UserProfiles")] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            var newconfiguration = new ConfigurationBuilder()
                                    .SetBasePath(context.FunctionAppDirectory)
                                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                    .AddJsonFile("localSecrets.settings.json", optional: true, reloadOnChange: true)
                                    .AddEnvironmentVariables()
                                    .Build();
            var storageaccountconnectionString = "DefaultEndpointsProtocol=https;AccountName=slazureautonumber;AccountKey=+NvGarHB994j8xGysbPYNaYuxw37IfKubrjAlW7vZPM9X9/88pD6+WB4r4PjCG5HWlgKTbtltX8o+AStzvbcUg==;EndpointSuffix=core.windows.net";
            //var CosmosDBConnectionStringhotmail = "AccountEndpoint=https://cosmodbadmin.documents.azure.com:443/;AccountKey=GwHeHkvSrF7iVsfRtHglDaB1tikWWIffXDxqCF2yyz2SeHP7kpypiVd6z3OjctKfzfzM1S2m3vMXACDbAgZInQ==;";
            var CosmosDBConnectionString  = "AccountEndpoint=https://dev-uppoc-acdb.documents.azure.com:443/;AccountKey=GfQGzQxuSXhUQFa5irQPKf1V2qytsrzUkpPkJIBcRewM0JwLKetuuB4x8ZOGu8PpzCocgUaGCxMhACDbBZ93VQ==;EndpointSuffix=core.windows.net";
            using CosmosClient client = new CosmosClient(CosmosDBConnectionString);

            if (req.Method == "GET")
            {
                try
                {
                    string id = req.Query["person_key"];
                    string recordId = req.Query["recordId"];

                    string email = req.Query["email"];
                    string firstName = req.Query["firstName"];
                    string lastName = req.Query["lastName"];
                    string phone = req.Query["phone"];

                    // New instance of CosmosClient class
                    //using CosmosClient client = new CosmosClient(newconfiguration.GetSection("CosmosDBConnectionString").Value);

                    //using CosmosClient client = new CosmosClient(CosmosDBConnectionString);


                    // Database reference with creation if it does not already exist
                    Database database = client.GetDatabase(id: "user_profile_db");

                    log.LogInformation("Get Database " + DateTime.Now.Ticks);

                    // Container reference with creation if it does not alredy exist
                    Microsoft.Azure.Cosmos.Container container = database.GetContainer(id: "user_profile");
                    log.LogInformation("Get Containter " + DateTime.Now.Ticks);

                    QueryDefinition query;
                    // Create query using a SQL string and parameters
                    if (id != null)
                    {
                        if (id == string.Empty)
                        {
                            return new ContentResult()
                            {
                                Content = "ID is empty",
                                ContentType = "appliation/json",
                                StatusCode = 400

                            };
                        }
                        else
                        {
                            query = new QueryDefinition(
                                  query: "SELECT * FROM c  WHERE c.person_key = @id"
                                  )
                              .WithParameter("@id", id);
                                }

                    }
                    else if (recordId != null)
                    {
                        if (id == string.Empty)
                        {
                            return new ContentResult()
                            {
                                Content = "Record ID is empty",
                                ContentType = "appliation/json",
                                StatusCode = 400

                            };
                        }
                        else
                        {
                            query = new QueryDefinition(
                                  query: "SELECT * FROM c  WHERE c.record_id = @recordId"
                                  )
                              .WithParameter("@recordId", recordId);
                        }

                    }
                    else if (firstName != null)
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
                            query = new QueryDefinition(
                                 query: "SELECT * FROM c  WHERE c.first_name = @firstName order by c.first_name"
                         )
                            .WithParameter("@firstName", firstName);

                            log.LogInformation("Get Data by First Name " + firstName  + " Time = "+ DateTime.Now.Ticks);

                        }

                    }

                    else if (lastName != null)
                    {
                        if (lastName == string.Empty)
                        {
                            return new ContentResult()
                            {
                                Content = "last name is empty",
                                ContentType = "appliation/json",
                                StatusCode = 400

                            };
                        }
                        else
                        {
                            query = new QueryDefinition(
                                  query: "SELECT * FROM c  WHERE c.last_name = @lastName"
                                  )
                              .WithParameter("@lastName", lastName);

                                    log.LogInformation("Get Data by Last Name " + lastName + " Time " + DateTime.Now.Ticks);

                        }

                    }

                    else if (email != null)
                    {
                        if (email == string.Empty)
                        {
                            return new ContentResult()
                            {
                                Content = "email is empty",
                                ContentType = "appliation/json",
                                StatusCode = 400

                            };
                        }
                        else
                        {

                             query = new QueryDefinition(
                             query: "SELECT c.person_key, c.id, c.first_name," +
                                       "c.last_name,c.record_id," +
                                       "c.profile FROM c JOIN zc IN c.profile.emails " +
                                       "WHERE zc.personal = @email"
                                     )
                                 .WithParameter("@email", email);

                            log.LogInformation("Get Data by Email = " + email + " Time " + DateTime.Now.Ticks);



                        }

                    }
                    else
                    {
                        query = new QueryDefinition(
                                    query: "SELECT * FROM c"
                            );

                    }


                    using FeedIterator<UserProfile> feed = container.GetItemQueryIterator<UserProfile>(
                                queryDefinition: query, 
                                null, 
                                
                                requestOptions: new QueryRequestOptions() {
                                    MaxConcurrency = -1,
                                    PopulateIndexMetrics = true,
                                    ConsistencyLevel = ConsistencyLevel.Session
                                                                         
                                }
                                
                               
                    );

                    log.LogInformation("Call GetItemQueryIterator " + DateTime.Now.Ticks);

                    List<UserProfile> users = new List<UserProfile>();
                    Stopwatch queryExecutionTimeEndToEndTotal = new Stopwatch();

                    while (feed.HasMoreResults)
                    {
                        queryExecutionTimeEndToEndTotal.Start();

                        FeedResponse<UserProfile> response = await feed.ReadNextAsync();
                        queryExecutionTimeEndToEndTotal.Stop();


                        log.LogInformation("feed ReadNextAsync " + DateTime.Now.Ticks);
                        log.LogInformation("response.RequestCharge =  " + response.RequestCharge);
                         

                        foreach (var item in response)
                        {
                            users.Add(item);
                        }
                        log.LogInformation("Call ReadNextAsync " + DateTime.Now.Ticks);

                    }
                    log.LogInformation("queryExecutionTimeEndToEndTotal (ms) =  " + queryExecutionTimeEndToEndTotal.ElapsedMilliseconds);


                    return new OkObjectResult(users);
                }
                catch (Exception ex)
                {
                    log.LogError("Error " + ex.Message);

                    return null;
                }

            }
            
            
            else
            {
                return new OkObjectResult("Error not GET");

            }

        }
    }
}
