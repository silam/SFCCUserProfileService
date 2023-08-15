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
    public class PostCosmosDbUserProfile
    {
        [FunctionName("PostCosmosDbUserProfile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "CosmosDb/UserProfiles")] HttpRequest req,
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

            if (req.Method == "POST")
            {
                try
                {
                    var blobServiceClient = new BlobServiceClient(storageaccountconnectionString);

                    var blobOptimisticDataStore = new BlobOptimisticDataStore(blobServiceClient, "unique-ids");

                    var idGen = new UniqueIdGenerator(blobOptimisticDataStore);

                    // generate ids with different scopes

                    //var record_id = idGen.NextId("SFCCUniversalProfile");


                    List<UserProfile> users = new List<UserProfile>();

                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                    //UserProfile data = JsonConvert.DeserializeObject<UserProfile>(requestBody);

                    List<UserProfile> dataLst = JsonConvert.DeserializeObject<List<UserProfile>>(requestBody);


                    // New instance of CosmosClient class
                    ///using CosmosClient client = new CosmosClient(newconfiguration.GetSection("CosmosDBConnectionString").Value);

                    // Database reference with creation if it does not already exist
                    Database database = client.GetDatabase(id: "user_profile_db");


                    // Container reference with creation if it does not alredy exist
                    Microsoft.Azure.Cosmos.Container container = database.GetContainer(id: "user_profile");


                    for (int i = 0; i < 1; i++)
                    {
                        dataLst = JsonConvert.DeserializeObject<List<UserProfile>>(requestBody);

                        foreach (var data in dataLst)
                        {
                           var record_id = idGen.NextId("SFCCUniversalProfile");

                            var record_id_str = record_id.ToString();
                            string person_key = data?.person_key + record_id_str;
                            string first_name = data?.first_name + record_id_str;
                            string last_name = data?.last_name + record_id_str;
                            Profile _profile = data?.profile;


                            Profile profile = new Profile();

                            foreach (var email in _profile.emails)
                            {
                                email.personal = record_id_str + email.personal;
                            }

                            foreach (var a in _profile.addresses)
                            {
                                a.delivery = record_id_str + " " + a.delivery;
                            }

                            foreach (var p in _profile.phones)
                            {
                                p.number = record_id_str + p.number;
                            }

                            profile = _profile;

                            UserProfile r = new UserProfile()
                            {
                                id = Guid.NewGuid().ToString(),
                                record_id = record_id.ToString(),
                                person_key = person_key,
                                first_name = first_name,
                                last_name = last_name,
                                profile = profile
                            };


                            UserProfile item = await container.CreateItemAsync(
                               item: r,
                               partitionKey: new PartitionKey(r.record_id.ToString())
                           );

                            System.Threading.Thread.Sleep(1);
                            Console.WriteLine("Record " + i);
                        }
                    }


                    return new OkObjectResult(dataLst);

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
