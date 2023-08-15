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
    public class DeleteCosmosDbUserProfile
    {
        [FunctionName("DeleteCosmosDbUserProfile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous,"delete", Route = "CosmosDb/UserProfiles")] HttpRequest req,
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

             if (req.Method == "DELETE")
            {
                try
                {
                    List<UserProfile> users = new List<UserProfile>();

                    string email = req.Query["email"];

                    //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                    //UserProfile data = JsonConvert.DeserializeObject<UserProfile>(requestBody);


                    //using CosmosClient client = new CosmosClient(newconfiguration.GetSection("CosmosDBConnectionString").Value);
                    Database database = client.GetDatabase(id: "user_profile_db");


                    Microsoft.Azure.Cosmos.Container container = database.GetContainer(id: "user_profile");

                    QueryDefinition query = new QueryDefinition(
                             query: "SELECT c.person_key, c.id, c.first_name," +
                                       "c.last_name,c.record_id," +
                                       "c.profile FROM c JOIN zc IN c.profile.emails " +
                                       "WHERE zc.personal = @email"
                                     )
                                 .WithParameter("@email", email);

                    log.LogInformation("Get Data by Email = " + email + " Time " + DateTime.Now.Ticks);
                    
                    using FeedIterator<UserProfile> feed = container.GetItemQueryIterator<UserProfile>(
                                queryDefinition: query,
                                null,

                                requestOptions: new QueryRequestOptions()
                                {
                                    MaxConcurrency = -1
                                }
                    );

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


                    if ( users != null && users.Count > 0)
                    {

                         

                        //Delete an item.Note we must provide the partition key value and id of the item to delete
                        ItemResponse<UserProfile> userResponse = await container.DeleteItemAsync<UserProfile>(users[0].id, new PartitionKey(users[0].record_id));
                        Console.WriteLine("Deleted   partitionKey and id [{0},{1}]\n", users[0].record_id, users[0].id);

                        return new OkObjectResult(new { message = "Item is deleted" });
                    }
                    return new OkObjectResult(new { message = "No Item deleted" });


                }
                catch (Exception e)
                {
                    var error = new { error = e.Message };
                    return new ContentResult()
                    {
                        Content = e.Message,
                        ContentType = "appliation/json",
                        StatusCode = 503

                    };
                }

            }
            
            else
            {
                return new OkObjectResult(null);

            }

        }
    }
}
