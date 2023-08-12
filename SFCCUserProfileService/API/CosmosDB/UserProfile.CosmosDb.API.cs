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


namespace SFCCUserProfileService.API.CosmosDB
{
    public class GetCosmosDbUserProfile
    {
        [FunctionName("CosmosDbUserProfiles")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "delete", "put", Route = "CosmosDb/UserProfiles")] HttpRequest req,
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
                                 query: "SELECT * FROM c  WHERE c.first_name = @firstName"
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
                        queryDefinition: query
                    );

                    log.LogInformation("Call GetItemQueryIterator " + DateTime.Now.Ticks);

                    List<UserProfile> users = new List<UserProfile>();

                    while (feed.HasMoreResults)
                    {
                        FeedResponse<UserProfile> response = await feed.ReadNextAsync();
                        log.LogInformation("feed ReadNextAsync " + DateTime.Now.Ticks);


                        foreach (var item in response)
                        {
                            users.Add(item);
                        }
                        log.LogInformation("Call ReadNextAsync " + DateTime.Now.Ticks);

                    }


                    return new OkObjectResult(users);
                }
                catch (Exception ex)
                {
                    log.LogError("Error " + ex.Message);

                    return null;
                }

            }
            else if (req.Method == "POST")
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
                catch (Exception ex)
                {
                    return null;
                }

            }
            else if (req.Method == "DELETE")
            {
                try
                {
                    List<UserProfile> users = new List<UserProfile>();

                    string record_id = req.Query["record_id"];

                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                    UserProfile data = JsonConvert.DeserializeObject<UserProfile>(requestBody);


                    //using CosmosClient client = new CosmosClient(newconfiguration.GetSection("CosmosDBConnectionString").Value);
                    Database database = client.GetDatabase(id: "user_profile_db");


                    Microsoft.Azure.Cosmos.Container container = database.GetContainer(id: "user_profile");
                    //ResponseMessage deleteResponse = await container.DeleteAllItemsByPartitionKeyStreamAsync(new PartitionKey("Contoso"));

                    // Delete an item. Note we must provide the partition key value and id of the item to delete
                    ItemResponse<UserProfile> userResponse = await container.DeleteItemAsync<UserProfile>(data.id, new PartitionKey(data.record_id));
                    Console.WriteLine("Deleted   partitionKey and id [{0},{1}]\n", record_id, record_id);

                    return new OkObjectResult(new { message = "Item is deleted" });
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
            else if (req.Method == "PUT")
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
                return new OkObjectResult(null);

            }

        }
    }
}
