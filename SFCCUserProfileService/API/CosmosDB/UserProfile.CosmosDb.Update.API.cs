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
    public class PutCosmosDbUserProfile
    {
        [FunctionName("PutCosmosDbUserProfile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "CosmosDb/UserProfiles")] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            var newconfiguration = new ConfigurationBuilder()
                                    .SetBasePath(context.FunctionAppDirectory)
                                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                    .AddJsonFile("localSecrets.settings.json", optional: true, reloadOnChange: true)
                                    .AddEnvironmentVariables()
                                    .Build();
            var CosmosDBConnectionString  = "AccountEndpoint=https://dev-uppoc-acdb.documents.azure.com:443/;AccountKey=GfQGzQxuSXhUQFa5irQPKf1V2qytsrzUkpPkJIBcRewM0JwLKetuuB4x8ZOGu8PpzCocgUaGCxMhACDbBZ93VQ==;EndpointSuffix=core.windows.net";
            using CosmosClient client = new CosmosClient(CosmosDBConnectionString);
            
            if (req.Method == "PUT")
            {
                try
                {
                    //using CosmosClient client = new CosmosClient(newconfiguration.GetSection("CosmosDBConnectionString").Value);
                    Database database = client.GetDatabase(id: "user_profile_db");
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                    UserProfile data = JsonConvert.DeserializeObject<UserProfile>(requestBody);
                    string id = data?.id;
                    string record_id = data?.record_id;
                    string first_name = data?.first_name;
                    string last_name = data?.last_name;


                    Microsoft.Azure.Cosmos.Container container = database.GetContainer(id: "user_profile");

                    ItemResponse<UserProfile> user = await container.ReadItemAsync<UserProfile>(id, new PartitionKey(record_id));
                    var itemBody = user.Resource;

                    // update FirstName
                    itemBody.first_name = first_name == null ? itemBody.first_name : first_name;
                    itemBody.last_name = last_name == null ? itemBody.last_name : last_name;

                    // replace/update the item with the updated content
                    ItemResponse<UserProfile> newUser = await container.ReplaceItemAsync(itemBody, itemBody.id, new PartitionKey(itemBody.record_id));

                    return new OkObjectResult(itemBody);
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
                return new OkObjectResult("Error not POST");

            }

        }
    }
}
