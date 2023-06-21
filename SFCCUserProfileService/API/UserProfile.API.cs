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

namespace SFCCUserProfileService.API
{
    public static class GetUserProfile
    {
        [FunctionName("UserProfiles")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "delete", "put", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
            var newconfiguration = new ConfigurationBuilder()
                                    .SetBasePath(context.FunctionAppDirectory)
                                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                    .AddJsonFile("localSecrets.settings.json", optional: true, reloadOnChange: true)
                                    .AddEnvironmentVariables()
                                    .Build();
            var storageaccountconnectionString = "DefaultEndpointsProtocol=https;AccountName=slazureautonumber;AccountKey=+NvGarHB994j8xGysbPYNaYuxw37IfKubrjAlW7vZPM9X9/88pD6+WB4r4PjCG5HWlgKTbtltX8o+AStzvbcUg==;EndpointSuffix=core.windows.net";


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
                    using CosmosClient client = new CosmosClient(newconfiguration.GetSection("CosmosDBConnectionString").Value);

                    // Database reference with creation if it does not already exist
                    Database database = client.GetDatabase(id: "user_profile");


                    // Container reference with creation if it does not alredy exist
                    Microsoft.Azure.Cosmos.Container container = database.GetContainer(id: "userprofile");

                    QueryDefinition query;
                    // Create query using a SQL string and parameters
                    if (id != null)
                    {
                        if ( id == string.Empty)
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
                                      query: "SELECT * FROM userprofile  WHERE userprofile.person_key = @id"
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
                                     query: "SELECT * FROM userprofile  WHERE userprofile.first_name = @firstName"
                             )
                         .WithParameter("@firstName", firstName);
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
                                      query: "SELECT * FROM userprofile  WHERE userprofile.last_name = @lastName"
                              )
                          .WithParameter("@lastName", lastName);
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
                                      query: " SELECT c.person_key, c.id, c.first_name," +
                                                "c.last_name,c.record_id, c.rwsc_employee ," +
                                                "c.profile FROM c JOIN zc IN c.profile.emails " +
                                                "WHERE zc.personal = @email"
                              )
                          .WithParameter("@email", email);


                        }
                       
                    }
                    else
                    {
                        query = new QueryDefinition(
                                    query: "SELECT * FROM userprofile"
                            );
                       
                    }


                    using FeedIterator<UserProfile> feed = container.GetItemQueryIterator<UserProfile>(
                        queryDefinition: query
                    );

                    List<UserProfile> users = new List<UserProfile>();

                    while (feed.HasMoreResults)
                    {
                        FeedResponse<UserProfile> response = await feed.ReadNextAsync();
                        foreach (var item in response)
                        {
                            users.Add(item);
                        }

                    }


                    return new OkObjectResult(users);
                }
                catch (Exception ex)
                {
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

                    var record_id = idGen.NextId("SFCCUniversalProfile");


                    List<UserProfile> users = new List<UserProfile>();

                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                    UserProfile data = JsonConvert.DeserializeObject<UserProfile>(requestBody);
                    string person_key = data?.person_key;
                    string first_name = data?.first_name;
                    string last_name = data?.last_name;
                    Profile _profile = data?.profile;


                    // New instance of CosmosClient class
                    using CosmosClient client = new CosmosClient(newconfiguration.GetSection("CosmosDBConnectionString").Value);

                    // Database reference with creation if it does not already exist
                    Database database = client.GetDatabase(id: "user_profile");


                    // Container reference with creation if it does not alredy exist
                    Microsoft.Azure.Cosmos.Container container = database.GetContainer(id: "userprofile");

                    List<Address> addresses = new List<Address>(

                    )
                    {
                        new Address()
                        {
                            city = "Hamel",
                                 postal_code = "55340",
                                 state = "MN",
                                 delivery = "1313 Mockingbird Lane",
                                 type = "shipping"
                        }
                    };


                    Profile profile = new Profile();

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

                    return new OkObjectResult(data);

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


                    using CosmosClient client = new CosmosClient(newconfiguration.GetSection("CosmosDBConnectionString").Value);
                    Database database = client.GetDatabase(id: "user_profile");


                    Microsoft.Azure.Cosmos.Container container = database.GetContainer(id: "userprofile");
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
                    using CosmosClient client = new CosmosClient(newconfiguration.GetSection("CosmosDBConnectionString").Value);
                    Database database = client.GetDatabase(id: "user_profile");
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                    UserProfile data = JsonConvert.DeserializeObject<UserProfile>(requestBody);
                    string id = data?.id;
                    string record_id = data?.record_id;
                    string first_name = data?.first_name;
                    string last_name = data?.last_name;


                    Microsoft.Azure.Cosmos.Container container = database.GetContainer(id: "userprofile");

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